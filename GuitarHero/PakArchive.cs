using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiscUtil.Conversion;
using MiscUtil.IO;

namespace GuitarHero
{
    public class PakArchive : IDisposable
    {
        private List<PakEntry> entries;
        private PakEntry terminator;
        private Stream pakStream;
        private Stream pabStream;
        private uint dataOffset;
        private bool hasPab = false;

        public IReadOnlyList<PakEntry> Entries => entries;

        private void ReadHeader()
        {
            using (EndianBinaryReader br = new EndianBinaryReader(
                      EndianBitConverter.Big,
                      new NonClosingStreamWrapper(pakStream)))
            {
                PakEntry entry;
                QbKey lastKey = new QbKey(".last");

                this.updateSuspended = true;
                do
                {
                    entry = PakEntry.ParseHeader(br, this);
                    this.entries.Add(entry);
                }
                while (!entry.FileType.Equals(lastKey));

                this.updateSuspended = false;

                this.entries.RemoveAt(this.entries.Count - 1);
                this.terminator = entry;
            }
        }

        public PakArchive(Stream pakStream) {
            this.pakStream = pakStream;
            this.entries = new List<PakEntry>();
            this.ReadHeader();
            this.entries.Sort((e1, e2) => (int)(e1.FileOffset - e2.FileOffset));

            this.dataOffset = this.entries.Count > 0 ? this.entries[0].FileOffset : 0x1000;
        }

        public PakArchive(Stream pakStream, Stream pabStream) : this(pakStream) {
            this.pabStream = pabStream;
            this.hasPab = true;
        }

        public PakEntry CreateEntry(string filePath, bool withEmbeddedFilename = false)
        {
            var type = Path.GetExtension(filePath);
            var shortName = Path.GetFileNameWithoutExtension(filePath);

            this.updateSuspended = true;

            PakEntry newEntry = new PakEntry(this)
                                {
                                    HeaderOffset = this.terminator.HeaderOffset,
                                    FileOffsetRelative = this.terminator.FileOffsetRelative,
                                    FileType = new QbKey(type),
                                    FileShortNameKey = new QbKey(shortName)
                                };

            if (withEmbeddedFilename)
            {
                newEntry.EmbeddedFilename = filePath;
            }
            else
            {
                newEntry.FileFullNameKey = new QbKey(filePath);
            }

            this.entries.Add(newEntry);
            moveEntriesAfter(this.entries.Count, (int)newEntry.HeaderLength);
            this.pakStream.Position = newEntry.HeaderOffset;

            using (var bw = new EndianBinaryWriter(EndianBitConverter.Big, new NonClosingStreamWrapper(this.pakStream)))
            {
                newEntry.WriteHeaderTo(bw);
                this.terminator.WriteHeaderTo(bw);
            }

            this.updateSuspended = false;

            return newEntry;
        }

        // This should be called with this.updateSuspended == true.  It cannot function without it so 
        // it sets it anyway.
        private void moveEntriesAfter(int index, int offset)
        {
            Debug.Assert(this.updateSuspended, "moveEntriesAfter was called without updates suspended");
            this.updateSuspended = true;
            if (offset == 0) return;

            for (int i = index + 1; i < entries.Count; i++)
            {
                // += doesn't work because of the long-uint implicit conversions
                this.entries[i].HeaderOffset = (uint) (this.entries[i].HeaderOffset + offset);
            }

            this.terminator.HeaderOffset = (uint) (this.terminator.HeaderOffset + offset);

            if (this.terminator.HeaderOffset + this.terminator.HeaderLength > this.dataOffset)
            {
                this.extendHeadersRegion();
            }
        }

        private void extendHeadersRegion()
        {
            uint minDataOffset = this.terminator.HeaderOffset + this.terminator.HeaderLength;
            uint newDataOffset = 0x1000 * (uint)Math.Ceiling(minDataOffset / (double)0x1000);
            uint dataOffsetDelta = newDataOffset - this.dataOffset;

            if (!this.hasPab)
            {
                // Move the data further forward in the file tgoo
                using (var temp = new FileStream(Path.GetTempFileName(), FileMode.Truncate))
                {
                    this.pakStream.Position = this.dataOffset;
                    this.pakStream.CopyTo(temp);
                    this.pakStream.Position = newDataOffset;
                    temp.Position = 0;
                    temp.CopyTo(this.pakStream);
                }
            }

            this.pakStream.Position = minDataOffset;

            var zeroes = new byte[dataOffsetDelta];
            this.pakStream.Write(zeroes, 0, (int) (dataOffsetDelta - this.terminator.HeaderLength));

            foreach (var e in this.entries) {
                e.FileOffsetRelative += dataOffsetDelta;
            }

            this.dataOffset = newDataOffset;
            this.terminator.FileOffsetRelative += 0x1000;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private bool updateSuspended = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing)
                {
                    this.pakStream?.Dispose();
                    this.pabStream?.Dispose();
                }
                
                entries = null;
                disposedValue = true;
            }
        }
        
        public void Dispose() {
            Dispose(true);
        }
        #endregion

        public void UpdateEntry(PakEntry pakEntry)
        {
            if (updateSuspended) return;

            var index = entries.FindIndex(x => x.HeaderOffset == pakEntry.HeaderOffset);
            var nextHeaderPos = index == entries.Count
                                    ? this.terminator.HeaderOffset
                                    : this.entries[index + 1].HeaderOffset;
            var oldLength = nextHeaderPos - pakEntry.HeaderOffset;

            if (oldLength != pakEntry.HeaderLength)
            {
                this.updateSuspended = true;
                moveEntriesAfter(index, (int)pakEntry.HeaderLength - (int)oldLength);
                this.updateSuspended = false;
            }

            this.pakStream.Position = pakEntry.HeaderOffset;

            using (var bw = new EndianBinaryWriter(EndianBitConverter.Big, new NonClosingStreamWrapper(this.pakStream)))
            {
                pakEntry.WriteHeaderTo(bw);
            }
        }
    }
}

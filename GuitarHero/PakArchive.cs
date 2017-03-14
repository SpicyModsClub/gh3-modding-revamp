using System;
using System.Collections.Generic;
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
        private FileStream pakStream;
        private FileStream pabStream;
        private long pabOffset = -1;

        public IReadOnlyList<PakEntry> Entries => entries;

        private void ReadHeader()
        {
            using (EndianBinaryReader br = new EndianBinaryReader(
                      EndianBitConverter.Big,
                      new NonClosingStreamWrapper(pakStream)))
            {
                PakEntry entry;
                QbKey lastKey = new QbKey(".last");

                do
                {
                    entry = PakEntry.ParseHeader(br, this);
                    this.entries.Add(entry);
                }
                while (!entry.FileType.Equals(lastKey));

                this.entries.RemoveAt(this.entries.Count - 1);
                this.terminator = entry;
            }
        }

        public PakArchive(FileStream pakStream) {
            this.pakStream = pakStream;
            this.entries = new List<PakEntry>();
            this.ReadHeader();
        }

        public PakArchive(FileStream pakStream, FileStream pabStream) : this(pakStream) {
            this.pabStream = pabStream;
            this.pabOffset = pakStream.Length;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


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
    }
}

using System;
using System.IO;

using MiscUtil.IO;

namespace GuitarHero
{
    /// <summary>
    /// Represents an entry in the header of a <see cref="PakArchive"/>
    /// </summary>
    public class PakEntry
    {
        #region Property backing fields
        private string _embeddedFilename;
        private QbKey _embeddedFilenameKey;
        private QbKey _fileFullNameKey;
        private uint _fileLength;
        private uint _fileOffsetRelative;
        private QbKey _fileShortNameKey;
        private QbKey _fileType;
        private PakEntryFlags _flags;
        private uint _unknown;
        #endregion

        private uint headerOffset;

        private PakArchive sourceArchive;

        internal PakEntry(PakArchive sourceArchive)
        {
            this.sourceArchive = sourceArchive;

            FileType = new QbKey(0);
            HeaderOffset = 0;
            FileOffsetRelative = 0;
            FileLength = 0;
            EmbeddedFilenameKey = new QbKey(0);
            FileFullNameKey = new QbKey(0);
            FileShortNameKey = new QbKey(0);
            Unknown = 0;
            this._flags = 0;
            this._embeddedFilename = null;
        }

        private void clearEmbeddedFilename()
        {
            this._embeddedFilename = null;

            if (this._fileFullNameKey.Checksum == 0)
            {
                this._fileFullNameKey = EmbeddedFilenameKey;
            }

            this._embeddedFilenameKey = new QbKey(0);
            this._flags &= ~PakEntryFlags.HasEmbeddedFilename;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="path">TODO</param>
        public void ExtractTo(string path)
        {
            throw new NotImplementedException();
        }

        private void NotifyArchive()
        {
            this.sourceArchive.UpdateEntry(this);
        }

        internal static PakEntry ParseHeader(EndianBinaryReader br, PakArchive sourceArchive)
        {
            var result = new PakEntry(sourceArchive);

            result.HeaderOffset = (UInt32)br.BaseStream.Position;
            result._fileType = new QbKey(br.ReadUInt32());
            result._fileOffsetRelative = br.ReadUInt32();
            result._fileLength = br.ReadUInt32();
            result._embeddedFilenameKey = new QbKey(br.ReadUInt32());
            result._fileFullNameKey = new QbKey(br.ReadUInt32());
            result._fileShortNameKey = new QbKey(br.ReadUInt32());
            result._unknown = br.ReadUInt32();
            result._flags = (PakEntryFlags)br.ReadUInt32();

            if (result.Flags.HasFlag(PakEntryFlags.HasEmbeddedFilename))
            {
                var embeddedFilenameBytes = br.ReadBytes(0xA0);
                result._embeddedFilename = Utility.Latin1Encoding.GetString(embeddedFilenameBytes).TrimEnd('\0');
            }

            return result;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void Remove()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="s">TODO</param>
        public void ReplaceData(Stream s)
        {
            throw new NotImplementedException();
        }

        internal void WriteHeaderTo(EndianBinaryWriter bw)
        {
            bw.Write(this.FileType.Checksum);
            bw.Write(this.FileOffsetRelative);
            bw.Write(this.FileLength);
            bw.Write(this.EmbeddedFilenameKey.Checksum);
            bw.Write(this.FileFullNameKey.Checksum);
            bw.Write(this.FileShortNameKey.Checksum);
            bw.Write(this.Unknown);
            bw.Write((UInt32)this.Flags);

            if (this.EmbeddedFilename != null)
            {
                var embedNameBytes = Utility.Latin1Encoding.GetBytes(this.EmbeddedFilename);
                bw.Write(embedNameBytes);
            }
        }

        /// <summary>
        /// The offset of this header, measured in bytes from the start of the PAK archive.
        /// </summary>
        internal UInt32 HeaderOffset
        {
            get
            {
                return this.headerOffset;
            }
            set
            {
                this.headerOffset = value;
            }
        }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file's type.
        /// </summary>
        /// <remarks>
        /// This value is normally the same as the file's extension including the
        /// dot, but this is not always the case.  For example, sound files in
        /// global_sfx have FileType ".wav" when they are actually mp2 files.
        /// </remarks>
        public QbKey FileType
        {
            get
            {
                return this._fileType;
            }
            set
            {
                this._fileType = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The offset of this entry's file, measured in bytes from the start of
        /// this header.
        /// </summary>
        /// <remarks>
        /// If the <see cref="PakArchive"/> has a separate data (PAB) component,
        /// the offset is measured as if the files are concatenated together.
        /// </remarks>
        internal UInt32 FileOffsetRelative
        {
            get
            {
                return this._fileOffsetRelative;
            }
            set
            {
                this._fileOffsetRelative = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The offset of this entry's file, measured in bytes from the start of
        /// the PAK archive.
        /// </summary>
        /// <remarks>
        /// If the <see cref="PakArchive"/> has a separate data (PAB) component,
        /// the offset is measured as if the files are concatenated together.
        /// </remarks>
        public UInt32 FileOffset => FileOffsetRelative + HeaderOffset;

        /// <summary>
        /// The length of this entry's file, measured in bytes.
        /// </summary>
        public UInt32 FileLength
        {
            get
            {
                return this._fileLength;
            }
            internal set
            {
                this._fileLength = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the <see cref="EmbeddedFilename"/>
        /// if it is present, otherwise the zero QbKey.
        /// </summary>
        public QbKey EmbeddedFilenameKey
        {
            get
            {
                return this._embeddedFilenameKey;
            }
            private set
            {
                this._embeddedFilenameKey = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file path for this entry
        /// relative to the pak archive.  If <see cref="EmbeddedFilename"/> is present,
        /// then this is the zero QbKey.
        /// </summary>
        public QbKey FileFullNameKey
        {
            get
            {
                return this._fileFullNameKey;
            }
            set
            {
                this._fileFullNameKey = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file name (excluding extension)
        /// for this entry.
        /// </summary>
        public QbKey FileShortNameKey
        {
            get
            {
                return this._fileShortNameKey;
            }
            set
            {
                this._fileShortNameKey = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// From tma's pak file spec:
        /// 0x18    DWORD   unknown (always zero?)
        /// </summary>
        public UInt32 Unknown
        {
            get
            {
                return this._unknown;
            }
            set
            {
                this._unknown = value;
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// Bitwise flags for how the file and header are treated within the
        /// <see cref="PakArchive"/>.  Other than the flag for whether an
        /// embedded filename is present, all are unknown.
        /// </summary>
        public PakEntryFlags Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                this._flags = value;
                if (value.HasFlag(PakEntryFlags.HasEmbeddedFilename))
                {
                    if (this._embeddedFilename == null)
                    {
                        this._embeddedFilename = "";
                        this._embeddedFilenameKey = this.FileFullNameKey;
                        this._fileFullNameKey = new QbKey(0);
                    }
                }
                else
                {
                    clearEmbeddedFilename();
                }
                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The entry's filename embedded directly in the header.  As the field
        /// is optional in the header, this can be null.
        /// </summary>
        public string EmbeddedFilename
        {
            get
            {
                return this._embeddedFilename;
            }
            set
            {
                if (value == null)
                {
                    this.clearEmbeddedFilename();
                }
                else
                {
                    this._embeddedFilename = value;
                    this._embeddedFilenameKey = new QbKey(value);
                    this._fileFullNameKey = new QbKey(0);
                    this._flags |= PakEntryFlags.HasEmbeddedFilename;

                    var shortName = Path.GetFileNameWithoutExtension(value);
                    this._fileShortNameKey = new QbKey(shortName);
                }

                this.NotifyArchive();
            }
        }

        /// <summary>
        /// The length of the header as stored in the <see cref="PakArchive"/>, measured in bytes.
        /// </summary>
        public uint HeaderLength => this._embeddedFilename == null ? 0x20u : 0xC0u;
    }

    [Flags]
    public enum PakEntryFlags : uint
    {
        HasEmbeddedFilename = 0x20
    }
}
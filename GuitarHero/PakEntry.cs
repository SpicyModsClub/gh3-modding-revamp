using System;

using MiscUtil.IO;

namespace GuitarHero
{
    /// <summary>
    /// Represents an entry in the header of a <see cref="PakArchive"/>
    /// </summary>
    public class PakEntry
    {
        private string _embeddedFilename;

        private PakEntryFlags _flags;

        private PakArchive sourceArchive;

        internal PakEntry(PakArchive sourceArchive)
        {
            this.sourceArchive = sourceArchive;

            FileType = new QbKey(0);
            FileOffset = 0x20;
            FileLength = 4;
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

            if (this.FileFullNameKey.Checksum == 0)
            {
                this.FileFullNameKey = EmbeddedFilenameKey;
            }

            this.EmbeddedFilenameKey = new QbKey(0);
            this._flags &= ~PakEntryFlags.HasEmbeddedFilename;
        }

        internal static PakEntry ParseHeader(EndianBinaryReader br, PakArchive sourceArchive)
        {
            var result = new PakEntry(sourceArchive);
            result.FileType = new QbKey(br.ReadUInt32());
            result.FileOffset = br.ReadUInt32();
            result.FileLength = br.ReadUInt32();
            result.EmbeddedFilenameKey = new QbKey(br.ReadUInt32());
            result.FileFullNameKey = new QbKey(br.ReadUInt32());
            result.FileShortNameKey = new QbKey(br.ReadUInt32());
            result.Unknown = br.ReadUInt32();
            result._flags = (PakEntryFlags)br.ReadUInt32();

            if (result.Flags.HasFlag(PakEntryFlags.HasEmbeddedFilename))
            {
                var embeddedFilenameBytes = br.ReadBytes(0xA0);
                result._embeddedFilename = Utility.Latin1Encoding.GetString(embeddedFilenameBytes).TrimEnd('\0');
            }

            return result;
        }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file's type.
        /// </summary>
        /// <remarks>
        /// This value is normally the same as the file's extension including the
        /// dot, but this is not always the case.  For example, sound files in
        /// global_sfx have FileType ".wav" when they are actually mp2 files.
        /// </remarks>
        public QbKey FileType { get; set; }

        /// <summary>
        /// The offset of this entry's file, measured in bytes from the start of
        /// this header.
        /// </summary>
        /// <remarks>
        /// If the <see cref="PakArchive"/> has a separate data (PAB) component,
        /// the offset is measured as if the files are concatenated together.
        /// </remarks>
        public UInt32 FileOffset { get; internal set; }

        /// <summary>
        /// The length of this entry's file, measured in bytes.
        /// </summary>
        public UInt32 FileLength { get; internal set; }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the <see cref="EmbeddedFilename"/>
        /// if it is present, otherwise the zero QbKey.
        /// </summary>
        public QbKey EmbeddedFilenameKey { get; private set; }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file path for this entry
        /// relative to the pak archive.  If <see cref="EmbeddedFilename"/> is present,
        /// then this is the zero QbKey.
        /// </summary>
        public QbKey FileFullNameKey { get; set; }

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file name (excluding extension)
        /// for this entry.
        /// </summary>
        public QbKey FileShortNameKey { get; set; }

        /// <summary>
        /// From tma's pak file spec:
        /// 0x18    DWORD   unknown (always zero?)
        /// </summary>
        public UInt32 Unknown { get; set; }

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
                if (!value.HasFlag(PakEntryFlags.HasEmbeddedFilename))
                {
                    clearEmbeddedFilename();
                }
            }
        }

        /// <summary>
        /// The entry's filename embedded directly in the header.  As the field
        /// is optional in the header, this can be null.
        /// </summary>
        public string EmbeddedFilename
        {
            get {  return this._embeddedFilename; }  
            set
            {
                if (value == null)
                {
                    this.clearEmbeddedFilename();
                }
                else
                {
                    this._embeddedFilename = value;
                    this.EmbeddedFilenameKey = new QbKey(value);
                    this.FileFullNameKey = new QbKey(0);
                }
            }
        }
    }

    [Flags]
    public enum PakEntryFlags : uint
    {
        HasEmbeddedFilename = 0x20
    }
}
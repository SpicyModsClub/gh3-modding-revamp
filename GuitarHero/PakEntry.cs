using System;

namespace GuitarHero
{
    /// <summary>
    /// Represents an entry in the header of a <see cref="PakArchive"/>
    /// </summary>
    public class PakEntry
    {
        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file's type.
        /// </summary>
        /// <remarks>
        /// This value is normally the same as the file's extension including the
        /// dot, but this is not always the case.  For example, sound files in
        /// global_sfx have FileType ".wav" when they are actually mp2 files.
        /// </remarks>
        public QbKey FileType;

        /// <summary>
        /// The offset of this entry's file, measured in bytes from the start of
        /// this header.
        /// </summary>
        /// <remarks>
        /// If the <see cref="PakArchive"/> has a separate data (PAB) component,
        /// the offset is measured as if the files are concatenated together.
        /// </remarks>
        public UInt32 FileOffset;

        /// <summary>
        /// The length of this entry's file, measured in bytes.
        /// </summary>
        public UInt32 FileLength;

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the <see cref="EmbeddedFilename"/>
        /// if it is present, otherwise the zero QbKey.
        /// </summary>
        public QbKey EmbeddedFilenameKey;

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file path for this entry
        /// relative to the pak archive.  If <see cref="EmbeddedFilename"/> is present,
        /// then this is the zero QbKey.
        /// </summary>
        public QbKey FileFullNameKey;

        /// <summary>
        /// The <see cref="QbKey"/> corresponding to the file name (excluding extension)
        /// for this entry.
        /// </summary>
        public QbKey FileShortNameKey;

        /// <summary>
        /// From tma's pak file spec:
        /// 0x18    DWORD   unknown (always zero?)
        /// </summary>
        public UInt32 Unknown;

        /// <summary>
        /// Bitwise flags for how the file and header are treated within the
        /// <see cref="PakArchive"/>.  Other than the flag for whether an
        /// embedded filename is present, all are unknown.
        /// </summary>
        public PakEntryFlags Flags;

        /// <summary>
        /// The entry's filename embedded directly in the header.  As the field
        /// is optional in the header, this can be null.
        /// </summary>
        public string EmbeddedFilename;
    }

    [Flags]
    public enum PakEntryFlags : uint
    {
        HasEmbeddedFilename = 0x20
    }
}
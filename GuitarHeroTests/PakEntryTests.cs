using MiscUtil.Conversion;
using MiscUtil.IO;

using NUnit.Framework;

namespace GuitarHero.Tests
{
    [TestFixture]
    public class PakEntryTests
    {
        [Test]
        public void ParseHeaderNoEmbeddedNameTest()
        {
            PakEntry entry;

            using (var sample = TestHelpers.OpenSample("PakEntryNoEmbeddedName.dat"))
            using (var br = new EndianBinaryReader(EndianBitConverter.Big, sample)) {
                entry = PakEntry.ParseHeader(br, null);
            }

            Assert.AreEqual(new QbKey(0xA7F505C4), entry.FileType);
            Assert.AreEqual(0x1000, entry.FileOffset);
            Assert.AreEqual(0xA4, entry.FileLength);
            Assert.AreEqual(new QbKey(0), entry.EmbeddedFilenameKey);
            Assert.AreEqual(new QbKey(0x99360D32), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey(0xF6E6D11A), entry.FileShortNameKey);
            Assert.AreEqual(0, entry.Unknown);
            Assert.AreEqual((PakEntryFlags)0, entry.Flags);
            Assert.IsNull(entry.EmbeddedFilename);
        }

        [Test]
        public void ParseHeaderEmbeddedNameTest() {
            PakEntry entry;

            using (var sample = TestHelpers.OpenSample("PakEntryEmbeddedName.dat"))
            using (var br = new EndianBinaryReader(EndianBitConverter.Big, sample))
            {
                entry = PakEntry.ParseHeader(br, null);
            }


            Assert.AreEqual(new QbKey(0xA7F505C4), entry.FileType);
            Assert.AreEqual(0x1000, entry.FileOffset);
            Assert.AreEqual(0xA4, entry.FileLength);
            Assert.AreEqual(new QbKey(0x99360D32), entry.EmbeddedFilenameKey);
            Assert.AreEqual(new QbKey(0), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey(0xF6E6D11A), entry.FileShortNameKey);
            Assert.AreEqual(0, entry.Unknown);
            Assert.AreEqual(PakEntryFlags.HasEmbeddedFilename, entry.Flags);
            Assert.AreEqual(@"pak\models\guitars\axeleg\axeleg_scripts.qb", entry.EmbeddedFilename);
        }

        [Test]
        public void SetFlagsNoEmbedNameTest()
        {
            PakEntry entry = new PakEntry(null)
                             {
                                 EmbeddedFilename = @"path\to\test_filename",
                                 FileShortNameKey = new QbKey("test_filename")
                             };

            entry.Flags = 0;

            Assert.AreEqual((PakEntryFlags)0, entry.Flags);
            Assert.IsNull(entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(0), entry.EmbeddedFilenameKey);
            Assert.AreEqual(new QbKey(@"path\to\test_filename"), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename"), entry.FileShortNameKey);
        }

        [Test]
        public void SetFlagsEmbedNameAlreadyUnsetTest()
        {
            PakEntry entry = new PakEntry(null)
            {
                FileFullNameKey = new QbKey(@"path\to\test_filename"),
                FileShortNameKey = new QbKey("test_filename")
            };

            entry.Flags = 0;

            Assert.AreEqual((PakEntryFlags) 0, entry.Flags);
            Assert.IsNull(entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(0), entry.EmbeddedFilenameKey);
            Assert.AreEqual(new QbKey(@"path\to\test_filename"), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename"), entry.FileShortNameKey);
        }

        [Test]
        public void SetFlagsEmbedNameTest()
        {
            PakEntry entry = new PakEntry(null)
                             {
                                 FileFullNameKey = new QbKey(@"path\to\test_filename"),
                                 FileShortNameKey = new QbKey("test_filename")
                             };

            entry.Flags = PakEntryFlags.HasEmbeddedFilename;
            
            Assert.AreEqual(PakEntryFlags.HasEmbeddedFilename, entry.Flags);
            Assert.IsNotNull(entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(0), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename"), entry.FileShortNameKey);
            Assert.AreEqual(new QbKey(@"path\to\test_filename"), entry.EmbeddedFilenameKey);
        }

        [Test]
        public void SetFlagsEmbedNameAlreadySetTest()
        {
            PakEntry entry = new PakEntry(null)
            {
                EmbeddedFilename = @"path\to\test_filename",
                FileShortNameKey = new QbKey("test_filename")
            };

            entry.Flags = PakEntryFlags.HasEmbeddedFilename;

            Assert.AreEqual(PakEntryFlags.HasEmbeddedFilename, entry.Flags);
            Assert.AreEqual(@"path\to\test_filename", entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(0), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename"), entry.FileShortNameKey);
            Assert.AreEqual(new QbKey(@"path\to\test_filename"), entry.EmbeddedFilenameKey);
        }

        [Test]
        public void SetEmbedNameNonNullTest()
        {
            PakEntry entry = new PakEntry(null)
            {
                FileFullNameKey = new QbKey(@"path\to\test_filename"),
                FileShortNameKey = new QbKey("test_filename")
            };

            entry.EmbeddedFilename = @"path\to\test_filename_new";

            Assert.AreEqual(PakEntryFlags.HasEmbeddedFilename, entry.Flags);
            Assert.AreEqual(@"path\to\test_filename_new", entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(0), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename_new"), entry.FileShortNameKey);
            Assert.AreEqual(new QbKey(@"path\to\test_filename_new"), entry.EmbeddedFilenameKey);
        }

        [Test]
        public void SetEmbedNameNullTest()
        {
            PakEntry entry = new PakEntry(null)
            {
                EmbeddedFilename = @"path\to\test_filename",
                FileShortNameKey = new QbKey("test_filename")
            };

            entry.EmbeddedFilename = null;

            Assert.AreEqual((PakEntryFlags)0, entry.Flags);
            Assert.IsNull(entry.EmbeddedFilename);
            Assert.AreEqual(new QbKey(@"path\to\test_filename"), entry.FileFullNameKey);
            Assert.AreEqual(new QbKey("test_filename"), entry.FileShortNameKey);
            Assert.AreEqual(new QbKey(0), entry.EmbeddedFilenameKey);
        }
    }
}

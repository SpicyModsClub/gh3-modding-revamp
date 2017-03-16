using NUnit.Framework;
using GuitarHero;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiscUtil.Conversion;
using MiscUtil.IO;

namespace GuitarHero.Tests
{
    [TestFixture]
    public class PakArchiveTests
    {
        private FileStream smallNoPabStream;
        private FileStream smallWithPabPakStream;
        private FileStream smallWithPabPabStream;
        private FileStream largeNoPabStream;
        private FileStream largeWithPabPakStream;
        private FileStream largeWithPabPabStream;

        [SetUp]
        public void PakArchiveSetUp()
        {
            this.smallNoPabStream = TestHelpers.CreateTempCopy("PakArchive.SmallNoPab.pak.xen");
            this.smallWithPabPakStream = TestHelpers.CreateTempCopy("PakArchive.SmallWithPab.pak.xen");
            this.smallWithPabPabStream = TestHelpers.CreateTempCopy("PakArchive.SmallWithPab.pab.xen");
            this.largeNoPabStream = TestHelpers.CreateTempCopy("PakArchive.LargeNoPab.pak.xen");
            this.largeWithPabPakStream = TestHelpers.CreateTempCopy("PakArchive.LargeWithPab.pak.xen");
            this.largeWithPabPabStream = TestHelpers.CreateTempCopy("PakArchive.LargeWithPab.pab.xen");
        }

        [TearDown]
        public void PakArchiveTearDown()
        {
            this.smallNoPabStream.Dispose();
            this.smallWithPabPakStream.Dispose();
            this.smallWithPabPabStream.Dispose();
            this.largeNoPabStream.Dispose();
            this.largeWithPabPakStream.Dispose();
            this.largeWithPabPabStream.Dispose();
        }
        
        [Test]
        public void PakArchiveNoPabTest() {
            using (PakArchive archive = new PakArchive(this.smallNoPabStream))
            {
                Assert.AreEqual(3, archive.Entries.Count);
                Assert.AreEqual(0x1000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x2000, archive.Entries[1].FileOffset);
                Assert.AreEqual(0x1113A0, archive.Entries[2].FileOffset);
            }
        }

        [Test]
        public void PakArchiveWithPabTest() {
            using (PakArchive archive = new PakArchive(this.smallWithPabPakStream, this.smallWithPabPabStream)) {
                Assert.AreEqual(3, archive.Entries.Count);
                Assert.AreEqual(0x1000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x2000, archive.Entries[1].FileOffset);
                Assert.AreEqual(0x1113A0, archive.Entries[2].FileOffset);
            }
        }
        
        [Test]
        public void CreateEntrySmallNoPabTest() {
            using (PakArchive archive = new PakArchive(this.smallNoPabStream))
            {
                archive.CreateEntry(@"test\new\entry.txt");

                Assert.AreEqual(4, archive.Entries.Count);
                Assert.AreEqual(0x1000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x13A760, archive.Entries[3].FileOffsetRelative);
                Assert.AreEqual(0x60, archive.Entries[3].HeaderOffset);

                PakEntry entry;

                this.smallNoPabStream.Position = 0x60;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.smallNoPabStream),
                    null);

                Assert.AreEqual(new QbKey(".txt"), entry.FileType);
                Assert.AreEqual(0x13A760, entry.FileOffsetRelative);
                Assert.AreEqual(new QbKey(@"test\new\entry.txt"), entry.FileFullNameKey);

                this.smallNoPabStream.Position = 0x80;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.smallNoPabStream),
                    null);

                Assert.AreEqual(new QbKey(".last"), entry.FileType);
                Assert.AreEqual(0x13A740, entry.FileOffsetRelative);
            }
        }

        [Test]
        public void CreateEntrySmallWithPabTest() {
            using (PakArchive archive = new PakArchive(this.smallWithPabPakStream, this.smallWithPabPabStream)) {
                archive.CreateEntry(@"test\new\entry.txt");

                Assert.AreEqual(4, archive.Entries.Count);
                Assert.AreEqual(0x1000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x13A760, archive.Entries[3].FileOffsetRelative);
                Assert.AreEqual(0x60, archive.Entries[3].HeaderOffset);

                PakEntry entry;

                this.smallWithPabPakStream.Position = 0x60;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.smallWithPabPakStream),
                    null);

                Assert.AreEqual(new QbKey(".txt"), entry.FileType);
                Assert.AreEqual(0x13A760, entry.FileOffsetRelative);
                Assert.AreEqual(new QbKey(@"test\new\entry.txt"), entry.FileFullNameKey);

                this.smallNoPabStream.Position = 0x80;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.smallWithPabPakStream),
                    null);

                Assert.AreEqual(new QbKey(".last"), entry.FileType);
                Assert.AreEqual(0x13A740, entry.FileOffsetRelative);
            }
        }

        [Test]
        public void CreateEntryLargeNoPabTest() {
            using (PakArchive archive = new PakArchive(this.largeNoPabStream)) {
                archive.CreateEntry(@"test\new\entry.txt");

                Assert.AreEqual(128, archive.Entries.Count);
                Assert.AreEqual(0x2000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x6DC60, archive.Entries.Last().FileOffsetRelative);
                Assert.AreEqual(0xFE0, archive.Entries.Last().HeaderOffset);

                PakEntry entry;

                this.largeNoPabStream.Position = 0xFE0;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.largeNoPabStream),
                    null);

                Assert.AreEqual(new QbKey(".txt"), entry.FileType);
                Assert.AreEqual(0x6DC60, entry.FileOffsetRelative);
                Assert.AreEqual(new QbKey(@"test\new\entry.txt"), entry.FileFullNameKey);

                this.largeNoPabStream.Position = 0x1000;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.largeNoPabStream),
                    null);

                Assert.AreEqual(new QbKey(".last"), entry.FileType);
                Assert.AreEqual(0x6DC40, entry.FileOffsetRelative);

                this.largeNoPabStream.Position = 0x2000;
                var bytes = new byte[0x20];
                var expected = new byte[]
                               {
                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xB4, 0x1C, 0x08, 0x02, 0x04, 0x10, 0x04, 0x08, 0x0C,
                                   0x0C, 0x08, 0x02, 0x04, 0x14, 0x02, 0x04, 0x0C, 0x10, 0x10, 0x0C, 0x00, 0x00, 0x20, 0x07, 0x00
                               };
                this.largeNoPabStream.Read(bytes, 0, 0x20);

                Assert.AreEqual(expected, bytes);
            }
        }

        [Test]
        public void CreateEntryLargeWithPabTest() {
            using (PakArchive archive = new PakArchive(this.largeWithPabPakStream, this.largeWithPabPabStream)) {
                archive.CreateEntry(@"test\new\entry.txt");

                Assert.AreEqual(128, archive.Entries.Count);
                Assert.AreEqual(0x2000, archive.Entries[0].FileOffset);
                Assert.AreEqual(0x6DC60, archive.Entries.Last().FileOffsetRelative);
                Assert.AreEqual(0xFE0, archive.Entries.Last().HeaderOffset);

                PakEntry entry;

                this.largeWithPabPakStream.Position = 0xFE0;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.largeWithPabPakStream),
                    null);

                Assert.AreEqual(new QbKey(".txt"), entry.FileType);
                Assert.AreEqual(0x6DC60, entry.FileOffsetRelative);
                Assert.AreEqual(new QbKey(@"test\new\entry.txt"), entry.FileFullNameKey);

                this.largeWithPabPakStream.Position = 0x1000;
                entry = PakEntry.ParseHeader(
                    new EndianBinaryReader(EndianBitConverter.Big, this.largeWithPabPakStream),
                    null);

                Assert.AreEqual(new QbKey(".last"), entry.FileType);
                Assert.AreEqual(0x6DC40, entry.FileOffsetRelative);
                Assert.AreEqual(0x2000, this.largeWithPabPakStream.Length);
                
            }
        }
    }
}
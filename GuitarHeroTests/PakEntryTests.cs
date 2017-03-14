using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            byte[] header = {
                                0xA7, 0xF5, 0x05, 0xC4, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0xA4, 0x00, 0x00, 0x00, 0x00,
                                0x99, 0x36, 0x0D, 0x32, 0xF6, 0xE6, 0xD1, 0x1A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 
                            };

            PakEntry entry;

            using (var ms = new MemoryStream())
            using (var br = new EndianBinaryReader(EndianBitConverter.Big, ms))
            {
                ms.Write(header, 0, 0x20);
                ms.Position = 0;
                entry = PakEntry.ParseHeader(br, null);
            }

            Assert.AreEqual(0xA7F505C4, entry.FileType.Checksum);
            Assert.AreEqual(0x1000, entry.FileOffset);
            Assert.AreEqual(0xA4, entry.FileLength);
            Assert.AreEqual(0, entry.EmbeddedFilenameKey.Checksum);
            Assert.AreEqual(0x99360D32, entry.FileFullNameKey.Checksum);
            Assert.AreEqual(0xF6E6D11A, entry.FileShortNameKey.Checksum);
            Assert.AreEqual(0, entry.Unknown);
            Assert.AreEqual((PakEntryFlags)0, entry.Flags);
            Assert.IsNull(entry.EmbeddedFilename);
        }

        [Test]
        public void ParseHeaderEmbeddedNameTest() {
            byte[] header = {
                                0xA7, 0xF5, 0x05, 0xC4, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0xA4, 0x99, 0x36, 0x0D, 0x32,
                                0x00, 0x00, 0x00, 0x00, 0xF6, 0xE6, 0xD1, 0x1A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20
                            };

            var embedName = @"pak\models\guitars\axeleg\axeleg_scripts.qb";
            var embedNameBytes = new byte[0xA0];
            Encoding.ASCII.GetBytes(embedName, 0, embedName.Length, embedNameBytes, 0);

            PakEntry entry;

            using (var ms = new MemoryStream())
            using (var br = new EndianBinaryReader(EndianBitConverter.Big, ms))
            {
                ms.Write(header, 0, 0x20);
                ms.Write(embedNameBytes, 0, 0xA0);
                ms.Position = 0;
                entry = PakEntry.ParseHeader(br, null);
            }


            Assert.AreEqual(0xA7F505C4, entry.FileType.Checksum);
            Assert.AreEqual(0x1000, entry.FileOffset);
            Assert.AreEqual(0xA4, entry.FileLength);
            Assert.AreEqual(0x99360D32, entry.EmbeddedFilenameKey.Checksum);
            Assert.AreEqual(0, entry.FileFullNameKey.Checksum);
            Assert.AreEqual(0xF6E6D11A, entry.FileShortNameKey.Checksum);
            Assert.AreEqual(0, entry.Unknown);
            Assert.AreEqual(PakEntryFlags.HasEmbeddedFilename, entry.Flags);
            Assert.AreEqual(embedName, entry.EmbeddedFilename);
        }
    }
}

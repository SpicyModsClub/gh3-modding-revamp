using NUnit.Framework;
using System;
using System.Data.HashFunction.CRCStandards;
using System.Text;

namespace GuitarHero.Tests
{
    [TestFixture]
    public class QbKeyTests
    {
        [Test, Repeat(50)]
        public void QbKeyUIntTest()
        {
            var random = TestContext.CurrentContext.Random;
            uint x = random.NextUInt();
            var qbKey = new QbKey(x);
            Assert.AreEqual(x, qbKey.Checksum);
        }

        [Test, Repeat(50)]
        public void QbKeyStringTestNoNormalize()
        {
            string s = TestContext.CurrentContext.Random.GetString();
            var latin1 = Encoding.GetEncoding("iso-8859-1");
            var qbKey = new QbKey(s, false);
            var crc = new CRC32().ComputeHash(latin1.GetBytes(s));

            Assert.AreEqual(~BitConverter.ToUInt32(crc,0), qbKey.Checksum);
        }

        [Test, Repeat(50)]
        public void QbKeyStringTestNormalize()
        {
            string s = TestContext.CurrentContext.Random.GetString();
            var latin1 = Encoding.GetEncoding("iso-8859-1");
            var qbKey = new QbKey(s, true);
            s = QbKey.Normalize(s);

            var crc = new CRC32().ComputeHash(latin1.GetBytes(s));

            Assert.AreEqual(~BitConverter.ToUInt32(crc, 0), qbKey.Checksum);
        }

        [Test]
        public void QbKeyStringTest2()
        {
            var qbKey = new QbKey("test");
            Assert.AreEqual(0x278081F3, qbKey.Checksum);
        }

        [Test, Repeat(50)]
        public void QbKeyNormalizeTest1()
        {
            // Lowercase string with no / is unchanged
            var n = (int) TestContext.CurrentContext.Random.NextUInt(40);
            var s = TestContext.CurrentContext.Random.GetString(n, "abcdefghijklmnopqrstuvwxyz");

            Assert.AreEqual(s, QbKey.Normalize(s));
        }

        [Test, Repeat(50)]
        public void QbKeyNormalizeTest2()
        {
            // Uppercase string with no / is made lowercase
            var n = (int) TestContext.CurrentContext.Random.NextUInt(40);
            var s = TestContext.CurrentContext.Random.GetString(n, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            Assert.AreEqual(s.ToLowerInvariant(), QbKey.Normalize(s));
        }

        [Test]
        public void QbKeyNormalizeTest3()
        {
            // Forward slash is replaced with backslash
            var s = "a/b";

            Assert.AreEqual("a\\b", QbKey.Normalize(s));
        }
    }
}
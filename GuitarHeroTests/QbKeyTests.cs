using NUnit.Framework;
using System;
using System.Data.HashFunction.CRCStandards;
using System.Text;

namespace GuitarHero.Tests
{
    [TestFixture]
    public class QbKeyTests
    {
        [Test]
        public void QbKeyUIntTest()
        {
            var random = TestContext.CurrentContext.Random;
            uint x = random.NextUInt();
            var qbKey = new QbKey(x);
            Assert.AreEqual(qbKey.Checksum, x);
        }

        [Test]
        public void QbKeyStringTest()
        {
            string s = TestContext.CurrentContext.Random.GetString();
            var latin1 = Encoding.GetEncoding("iso-8859-1");
            var qbKey = new QbKey(s);
            var crc = new CRC32().ComputeHash(latin1.GetBytes(s));

            Assert.AreEqual(qbKey.Checksum, ~BitConverter.ToUInt32(crc,0));
        }

        [Test]
        public void QbKeyStringTest2()
        {
            var qbKey = new QbKey("test");
            Assert.AreEqual(qbKey.Checksum, 0x278081F3);
        }
    }
}
﻿using System;
using System.Data.HashFunction;
using System.Text;

namespace GuitarHero
{
    /// <summary>
    /// Represents an identifier, used widely as a key for a hash map lookup.
    /// </summary>
    /// <remarks>
    /// Guitar Hero 3 implements its name lookup as a hash map on the low-order 7 bits
    /// of the identifier's CRC (crc &amp; 0x7FFF), with collisions resolved by searching
    /// the bucket for the full CRC.
    /// 
    /// In the game files, identifiers are reduced to their checksums.  The crc is calculated
    /// as CRC32 but with the final xor omitted.
    /// </remarks>
    public class QbKey
    {
        private static readonly Encoding Latin1;
        private static readonly CRC CrcGen;

        /// <summary>
        /// The CRC of this identifier
        /// </summary>
        public UInt32 Checksum { get; private set; }

        static QbKey()
        {
            var crcSettings = new CRC.Setting(32, CRC.DefaultSettings.Polynomial, 0xFFFFFFFF, true, true, 0);
            CrcGen = new CRC(crcSettings);
            Latin1 = Encoding.GetEncoding("iso-8859-1");
        }

        /// <summary>
        /// Create a <see cref="QbKey"/> from a checksum.  This should only be used when the corresponding
        /// identifier's name is unknown.
        /// </summary>
        /// <param name="checksum">The identifier's checksum</param>
        public QbKey(uint checksum)
        {
            this.Checksum = checksum;
        }

        /// <summary>
        /// Create a <see cref="QbKey"/> from a named identifier.
        /// </summary>
        /// <param name="name">The identifier's name</param>
        public QbKey(string name)
        {
            var bytes = Latin1.GetBytes(name);
            this.Checksum = BitConverter.ToUInt32(CrcGen.ComputeHash(bytes), 0);
        }
    }
}
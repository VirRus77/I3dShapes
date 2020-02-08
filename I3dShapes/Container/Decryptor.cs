using System;
using System.Collections.Generic;

namespace I3dShapes.Container
{
    /// <summary>
    /// Created by "high" https://facepunch.com/member.php?u=60704
    /// </summary>
    internal partial class Decryptor
    {
        /// <summary>
        /// Key by seed
        /// </summary>
        private readonly uint[] _key;

        public Decryptor(byte seed)
        {
            _key = new uint[0x10];
            var startIdx = seed << 4;
            for (var i = 0; i < _key.Length; i++)
                _key[i] = KeyConst[startIdx + i];
            //Block Counter
            _key[0x8] = 0;
            _key[0x9] = 0;
        }

        private static void CopyTo(IReadOnlyList<byte> src, int sourceIndex, IList<uint> destination)
        {
            for (int i = sourceIndex,
                o = 0;
                o < destination.Count && i < src.Count;
                i += 4, o++)
            {
                destination[o] = (uint) ((src[i + 3] << 24) | (src[i + 2] << 16) | (src[i + 1] << 8) | src[i]);
            }
        }

        private static void CopyTo(IReadOnlyList<uint> source, int sourceIndex, IList<byte> destination)
        {
            if (sourceIndex >= source.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            }
            if (destination.Count < source.Count - sourceIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(destination));
            }
            if (destination.Count % 4 != 0)
            {
                throw new InvalidCastException(nameof(destination));
            }

            //Array.Copy(BitConverter.GetBytes(source[0]), 0, destination, 0);
            //Enumerable.Range(0,source.Count - sourceIndex)
            //          .Select(v=>new[]{})
            //          .ForEach(
            //              i =>
            //              {
            //                  destination[i*4+0] = 
            //              });
            for (int i = sourceIndex,
                o = 0;
                o < destination.Count && i < source.Count;
                i++, o += 4)
            {
                destination[o] = (byte) source[i];
                destination[o + 1] = (byte) (source[i] >> 8);
                destination[o + 2] = (byte) (source[i] >> 16);
                destination[o + 3] = (byte) (source[i] >> 24);
            }
        }

        private static uint Rol(uint val, int bits)
        {
            return (val << bits) | (val >> (32 - bits));
        }

        private static uint Ror(uint val, int bits)
        {
            return (val >> bits) | (val << (32 - bits));
        }

        private static void Shuffle1(uint[] key, int idx1, int idx2, int idx3, int idx4)
        {
            key[idx3] ^= Rol(key[idx2] + key[idx1], 7);
            key[idx4] ^= Rol(key[idx3] + key[idx1], 9);
            key[idx2] ^= Rol(key[idx3] + key[idx4], 13);
            key[idx1] ^= Ror(key[idx2] + key[idx4], 14);
        }

        private static void Shuffle2(uint[] key, int idx1, int idx2, int idx3, int idx4)
        {
            key[idx3] ^= Rol(key[idx2] + key[idx1], 7);
            key[idx4] ^= Rol(key[idx2] + key[idx3], 9);
            key[idx1] ^= Rol(key[idx3] + key[idx4], 13);
            key[idx2] ^= Ror(key[idx4] + key[idx1], 14);
        }

        internal static int RoundUpTo(int val, int toNearest)
        {
            if (toNearest <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(toNearest));
            }

            return ((val + toNearest - 1) / toNearest) * toNearest;
        }

        internal static ulong RoundUpTo(ulong value, ulong toNearest)
        {
            if (toNearest <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(toNearest));
            }

            return ((value + toNearest - 1) / toNearest) * toNearest;
        }
    }
}

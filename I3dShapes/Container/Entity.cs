﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using I3dShapes.Tools;

namespace I3dShapes.Container
{
    /// <summary>
    /// Entity.
    /// </summary>
    public class Entity : IDisposable
    {
        /// <summary>
        /// Entity type.
        /// </summary>
        public uint Type { get; private set; }

        /// <summary>
        /// Entity type.
        /// </summary>
        public uint Size { get; private set; }

        /// <summary>
        /// Index Decrypt Block.
        /// </summary>
        public ulong DecryptIndexBlock { get; private set; }

        /// <summary>
        /// Offset by start entity.
        /// </summary>
        public long OffsetRawBlock { get; private set; }

        /// <summary>
        /// Read meta information <see cref="Entity"/>
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="decryptor">Decryptor.</param>
        /// <param name="decryptIndexBlock">Index Decrypt Block.</param>
        /// <param name="endian">File endian.</param>
        /// <param name="version">File version.</param>
        /// <returns></returns>
        public static Entity Read(in Stream stream, in IDecryptor decryptor, ref ulong decryptIndexBlock, in Endian endian)
        {
            var cryptBlockCount = 0ul;

            var type = FileContainer.ReadDecryptUInt32(stream, decryptor, decryptIndexBlock + cryptBlockCount, endian);
            var blockSize = (uint)Marshal.SizeOf(type);
            cryptBlockCount += FileContainer.RoundUp(blockSize, Decryptor.CryptBlockSize);

            var size = FileContainer.ReadDecryptUInt32(stream, decryptor, decryptIndexBlock + cryptBlockCount, endian);
            blockSize = (uint)Marshal.SizeOf(size);
            cryptBlockCount += FileContainer.RoundUp(blockSize, Decryptor.CryptBlockSize);
            var startDecryptIndexBlock = decryptIndexBlock + cryptBlockCount;

            var offset = stream.Position;

            cryptBlockCount += (size + Decryptor.CryptBlockSize - 1) / Decryptor.CryptBlockSize;
            stream.Seek(size, SeekOrigin.Current);

            decryptIndexBlock += cryptBlockCount;
            return new Entity
            {
                Type = type,
                Size = size,
                OffsetRawBlock = offset,
                DecryptIndexBlock = startDecryptIndexBlock,
            };
        }

        public void Dispose()
        {
        }
    }
}
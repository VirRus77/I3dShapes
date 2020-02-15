using System;
using System.IO;

namespace I3dShapes.Tools.Extensions
{
    /// <summary>
    /// Extensions by <see cref="BinaryReader"/>.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Align current position by <see cref="countBytes"/>.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        /// <param name="countBytes">Align.</param>
        /// <returns>Readed align bytes.</returns>
        public static byte[] Align(this BinaryReader reader, int countBytes)
        {
            var mod = reader.Position() % countBytes;
            if (mod == 0)
            {
                return Array.Empty<byte>();
            }

            var bytesToRead = (int)(countBytes - mod);
            return reader.ReadBytes(bytesToRead);
        }

        /// <summary>
        /// Position the <see cref="BinaryReader.BaseStream"/>.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        /// <returns>Position within the <see cref="BinaryReader.BaseStream"/>.</returns>
        public static long Position(this BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        /// <summary>
        /// Length the <see cref="BinaryReader.BaseStream"/>.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        /// <returns>Length within the <see cref="BinaryReader.BaseStream"/>.</returns>
        public static long Length(this BinaryReader reader)
        {
            return reader.BaseStream.Length;
        }

        /// <summary>
        /// Read everything to the end <see cref="BinaryReader.BaseStream"/>.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        /// <returns>Readed bytes.</returns>
        /// <exception cref="Exception"></exception>
        public static byte[] ReadAll(this BinaryReader reader)
        {
            var size = reader.Length() - reader.Position();

            if (size > Int32.MaxValue)
            {
                throw new Exception($"Size big: {size}.");
            }

            return reader.ReadBytes((int)size);
        }

        /// <summary>
        /// Check end of <see cref="BinaryReader.BaseStream"/>.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        /// <returns>true - end of <inheritdoc cref="BinaryReader.BaseStream"/>, false - otherwise.</returns>
        public static bool EndOfStream(this BinaryReader reader)
        {
            return reader.BaseStream.Position == reader.BaseStream.Length;
        }
    }
}

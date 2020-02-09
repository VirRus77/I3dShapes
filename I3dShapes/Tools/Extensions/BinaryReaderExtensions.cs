using System;
using System.IO;

namespace I3dShapes.Tools.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static byte[] Align(this BinaryReader reader, int countBytes)
        {
            var mod = reader.BaseStream.Position % countBytes;
            if (mod == 0)
            {
                return Array.Empty<byte>();
            }

            var bytesToRead = (int)(countBytes - mod);
            return reader.ReadBytes(bytesToRead);
        }

        public static long Position(this BinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        public static long Length(this BinaryReader reader)
        {
            return reader.BaseStream.Length;
        }

        //public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin)
        //{
        //    if (!reader.BaseStream.CanSeek)
        //    {
        //        throw new Exception("Cant seek.");
        //    }
        //    return reader.BaseStream.Seek(offset, origin);
        //}

        public static byte[] ReadAll(this BinaryReader reader)
        {
            var size = reader.Length() - reader.Position();

            if (size > Int32.MaxValue)
            {
                throw new Exception($"Size big: {size}.");
            }

            return reader.ReadBytes((int)size);
        }

        public static bool EndOfStream(this BinaryReader reader)
        {
            return reader.BaseStream.Position == reader.BaseStream.Length;
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace I3dShapes.Tools
{
    /// <summary>
    /// <inheritdoc cref="BinaryReader"/>
    /// </summary>
    public class EndianBinaryReader : BinaryReader
    {
        private readonly Endian _endian;

        public EndianBinaryReader(Stream input, in Endian endian = Endian.Little, bool leaveOpen = false)
            : base(input, Encoding.ASCII, leaveOpen)
        {
            _endian = endian;
        }

        public EndianBinaryReader(Stream input, Encoding encoding, in Endian endian = Endian.Little)
            : base(input, encoding)
        {
            _endian = endian;
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen, in Endian endian = Endian.Little)
            : base(input, encoding, leaveOpen)
        {
            _endian = endian;
        }

        public byte[] Read(in int byteCount)
        {
            var buffer = new byte[byteCount];
            var read = 0;
            do
            {
                var offset = Read(buffer, read, byteCount - read);
                if (offset == 0)
                {
                    throw new EndOfStreamException();
                }

                read += offset;
            } while (read < byteCount);

            return buffer;
        }

        // ReSharper disable BuiltInTypeReferenceStyle
        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadInt16"/>
        /// </summary>
        /// <returns></returns>
        public override Int16 ReadInt16() => BitConverter.ToInt16(Swipe(Read(sizeof(Int16)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadUInt16"/>
        /// </summary>
        /// <returns></returns>
        public override UInt16 ReadUInt16() => BitConverter.ToUInt16(Swipe(Read(sizeof(UInt16)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadInt32"/>
        /// </summary>
        /// <returns></returns>
        public override Int32 ReadInt32() => BitConverter.ToInt32(Swipe(Read(sizeof(Int32)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadUInt32"/>
        /// </summary>
        /// <returns></returns>
        public override UInt32 ReadUInt32() => BitConverter.ToUInt32(Swipe(Read(sizeof(UInt32)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadInt64"/>
        /// </summary>
        /// <returns></returns>
        public override Int64 ReadInt64() => BitConverter.ToInt64(Swipe(Read(sizeof(Int64)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadUInt64"/>
        /// </summary>
        /// <returns></returns>
        public override UInt64 ReadUInt64() => BitConverter.ToUInt64(Swipe(Read(sizeof(UInt64)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadSingle"/>
        /// </summary>
        /// <returns></returns>
        public override Single ReadSingle() => BitConverter.ToSingle(Swipe(Read(sizeof(Single)), _endian), 0);

        /// <summary>
        /// <inheritdoc cref="BinaryReader.ReadDouble"/>
        /// </summary>
        /// <returns></returns>
        public override Double ReadDouble() => BitConverter.ToDouble(Swipe(Read(sizeof(Double)), _endian), 0);
        // ReSharper restore BuiltInTypeReferenceStyle

        private static byte[] Swipe(byte[] read, in Endian endian)
        {
            if (BitConverter.IsLittleEndian && endian == Endian.Big)
            {
                Array.Reverse(read, 0, read.Length);
            }

            return read;
        }
    }
}

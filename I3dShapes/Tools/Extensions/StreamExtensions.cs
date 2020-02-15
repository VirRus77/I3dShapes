using System;
using System.IO;

namespace I3dShapes.Tools.Extensions
{
    /// <summary>
    /// Extensions by <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Read buffer.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="count">Byte count.</param>
        /// <returns>Byte buffer.</returns>
        /// <exception cref="EndOfStreamException">If the count is greater than the remaining bytes.</exception>
        public static byte[] Read(this Stream stream, in int count)
        {
            var buffer = new byte[count];
            var offset = 0;
            do
            {
                var read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += read;
            } while (offset < count);

            return buffer;
        }

        // ReSharper disable BuiltInTypeReferenceStyle
        /// <summary>
        /// Read <see cref="Int16"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static Int16 ReadInt16(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToInt16(Swipe(stream.Read(sizeof(Int16)), endian), 0);

        /// <summary>
        /// Read <see cref="UInt16"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static UInt16 ReadUInt16(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToUInt16(Swipe(stream.Read(sizeof(UInt16)), endian), 0);

        /// <summary>
        /// Read <see cref="Int32"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static Int32 ReadInt32(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToInt32(Swipe(stream.Read(sizeof(Int32)), endian), 0);

        /// <summary>
        /// Read <see cref="UInt32"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static UInt32 ReadUInt32(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToUInt32(Swipe(stream.Read(sizeof(UInt32)), endian), 0);

        /// <summary>
        /// Read <see cref="Int64"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static Int64 ReadInt64(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToInt64(Swipe(stream.Read(sizeof(Int64)), endian), 0);

        /// <summary>
        /// Read <see cref="UInt64"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static UInt64 ReadUInt64(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToUInt64(Swipe(stream.Read(sizeof(UInt64)), endian), 0);

        /// <summary>
        /// Read <see cref="Single"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static Single ReadSingle(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToSingle(Swipe(stream.Read(sizeof(Single)), endian), 0);

        /// <summary>
        /// Read <see cref="Double"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns></returns>
        public static Double ReadDouble(this Stream stream, Endian endian = Endian.Little) =>
            BitConverter.ToDouble(Swipe(stream.Read(sizeof(Double)), endian), 0);
        // ReSharper restore BuiltInTypeReferenceStyle

        /// <summary>
        /// Swipe bytes by <see cref="Endian"/>.
        /// </summary>
        /// <param name="read">Swipe buffer</param>
        /// <param name="endian"><inheritdoc cref="Endian"/></param>
        /// <returns>Swiped buffer.</returns>
        private static byte[] Swipe(byte[] read, Endian endian)
        {
            if (BitConverter.IsLittleEndian && endian == Endian.Big)
            {
                Array.Reverse(read, 0, read.Length);
            }

            return read;
        }
    }
}

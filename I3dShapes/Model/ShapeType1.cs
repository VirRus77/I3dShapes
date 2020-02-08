using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I3dShapes.Exceptions;
using I3dShapes.Model.Primitive;
using I3dShapes.Tools.Extensions;

namespace I3dShapes.Model
{
    public class ShapeType1 : NamedShapeObject
    {
        [Flags]
        public enum UnknownFlag : uint
        {
            Flag1 = 0x01,
            Flag2 = 0x02,
            Flag3 = 0x04,
            Flag4 = 0x08,
            Flag5 = 0x10,
            Flag6 = 0x20,
            Flag7 = 0x40,
            Flag8 = 0x80,
            Flag9 = 0x0100,
            Flag10 = 0x0200,
            Flag11 = 0x0400,
            Flag12 = 0x0800,
            Flag13 = 0x1000,
            Flag14 = 0x2000,
            Flag15 = 0x4000,
            Flag16 = 0x8000,
        }

        public enum UnknownFlag2 : short
        {
            Flag1 = 0x01,
            Flag2 = 0x02,
            Flag3 = 0x04,
            Flag4 = 0x08,
            Flag5 = 0x10,
            Flag6 = 0x20,
            Flag7 = 0x40,
            Flag8 = 0x80,
        }

        public ShapeType1(BinaryReader reader)
            : base(ShapeType.Type1)
        {
            Load(reader);
        }

        public float BoundingVolumeX { get; private set; }
        public float BoundingVolumeY { get; private set; }
        public float BoundingVolumeZ { get; private set; }
        public float BoundingVolumeR { get; private set; }
        public int VertexCount { get; private set; }
        public int Unknown6 { get; private set; }
        public int Vertices { get; private set; }
        public UnknownFlag UnknownFlags { get; private set; }
        public int Unknown8 { get; private set; }
        public int UvCount { get; private set; }
        public int Unknown9 { get; private set; }
        public int VertexCount2 { get; private set; }

        public ICollection<PointIndex> PointIndexes { get; private set; }
        public ICollection<PointVector> PointVectors { get; private set; }

        /// <summary>
        /// Set if <see cref="Unknown6"/> = 2. Size 4 UInt32.
        /// </summary>
        public ICollection<uint> UnknownStruct6 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag1"> set. Size = 3 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData1 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag8"> set. Size = 4 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData8 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag2"> set. Size = 2 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData2 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag3"> set. Size = 2 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData3 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag4"> set. Size = 2 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData4 { get; private set; }

        /// <summary>
        /// Set if <see cref="UnknownFlag1.Flag6"> set. Size = 4 * <see cref="Vertices"/>
        /// </summary>
        public ICollection<float> UnknownData6 { get; private set; }

        public Additions Addition { get; set; }

        private new void Load(BinaryReader reader)
        {
            base.Load(reader);
            var pos = reader.BaseStream.Position;
            BoundingVolumeX = reader.ReadSingle();
            BoundingVolumeY = reader.ReadSingle();
            BoundingVolumeZ = reader.ReadSingle();
            BoundingVolumeR = reader.ReadSingle();
            VertexCount = reader.ReadInt32();
            Unknown6 = reader.ReadInt32();
            Vertices = reader.ReadInt32();
            UnknownFlags = (UnknownFlag) reader.ReadUInt32();
            Unknown8 = reader.ReadInt32();
            UvCount = reader.ReadInt32();
            Unknown9 = reader.ReadInt32();
            VertexCount2 = reader.ReadInt32();

            if (Unknown6 == 2)
            {
                UnknownStruct6 = Enumerable
                                 .Range(0, 4)
                                 .Select(v => reader.ReadUInt32())
                                 .ToArray();
            }
            else if (Unknown6 != 1)
            {
                throw new UnknownFormatShapeException();
            }

            PointIndexes = Enumerable
                           .Range(0, VertexCount / 3)
                           .Select(v => new PointIndex(reader, Vertices > 0xFFFF))
                           .ToArray();

            var align = reader.Align(4);
            if (align.Any(v => v != 0))
            {
                throw new UnknownFormatShapeException();
            }

            PointVectors = Enumerable
                           .Range(0, Vertices)
                           .Select(v => new PointVector(reader))
                           .ToArray();

            LoadBinary(reader, UnknownFlags, Vertices);
            Addition = new Additions(reader);

            if (!reader.EndOfStream())
            {
                throw new UnknownFormatShapeException();
            }
        }

        private void LoadBinary(BinaryReader reader, UnknownFlag flag, int verticesCount)
        {
            var pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag1))
            {
                UnknownData1 = Enumerable
                               .Range(0, verticesCount * 3)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag8))
            {
                UnknownData8 = Enumerable
                               .Range(0, verticesCount * 4)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag2))
            {
                UnknownData2 = Enumerable
                               .Range(0, verticesCount * 2)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag3))
            {
                UnknownData3 = Enumerable
                               .Range(0, verticesCount * 2)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag4))
            {
                UnknownData4 = Enumerable
                               .Range(0, verticesCount * 2)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
            if (flag.HasFlag(UnknownFlag.Flag6))
            {
                UnknownData6 = Enumerable
                               .Range(0, verticesCount * 4)
                               .Select(v => reader.ReadSingle())
                               .ToArray();
            }

            pos = reader.BaseStream.Position;
        }
    }
}

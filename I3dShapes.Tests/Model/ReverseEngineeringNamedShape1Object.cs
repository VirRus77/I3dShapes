using System;
using System.Collections;
using System.IO;
using System.Linq;
using I3dShapes.Model;
using I3dShapes.Tools;

namespace I3dShapes.Tests.Model
{
    public class ReverseEngineeringNamedShape1Object : NamedShapeObject
    {
        public ReverseEngineeringNamedShape1Object(uint rawType, byte[] rawData, Endian endian)
            : base(ShapeType.Unknown)
        {
            RawType = rawType;
            using var stream = new MemoryStream(rawData);
            using var reader = new EndianBinaryReader(stream, endian);
            Load(reader);
        }

        public ReverseEngineeringNamedShape1Object(uint rawType, BinaryReader reader)
            : base(ShapeType.Unknown)
        {
            RawType = rawType;
            Load(reader);
        }

        public uint RawType { get; }

        public byte[] RawData { get; private set; }

        public string Flag { get; private set; }
        public short Flag7_1 { get; private set; }
        public short Flag7_2 { get; private set; }

        private void Load(BinaryReader reader)
        {
            base.Load(reader);
            RawData = reader.ReadBytes((int) (reader.BaseStream.Length - reader.BaseStream.Position));
            var flag = BitConverter.ToUInt32(RawData, 28);
            var bitArray = new BitArray(BitConverter.GetBytes(flag));
            Flag = string.Join(
                "",
                Enumerable.Range(0, bitArray.Count)
                          .Select((v, i) => $"{(bitArray.Get(v) ? 1 : 0)}{((i + 1) % 8 == 0 ? "_" : "")}")
            );
            Flag = Flag.Substring(0, Flag.Length - 1);
        }
    }
}

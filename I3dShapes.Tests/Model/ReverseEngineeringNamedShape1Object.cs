using System;
using System.Collections;
using System.IO;
using System.Linq;
using I3dShapes.Model;
using I3dShapes.Tools;

namespace I3dShapes.Tests.Model
{
    /// <summary>
    /// Raw shape contains <see cref="ShapeType1.ContainsFlags"/> as string.
    /// </summary>
    public class ReverseEngineeringNamedShape1Object : RawNamedShapeObject
    {
        public ReverseEngineeringNamedShape1Object(uint rawType, BinaryReader reader, Endian endian)
            : base(rawType, reader, endian)
        {
            Load();
        }

        /// <summary>
        /// <see cref="ShapeType1.ContainsFlags"/>
        /// </summary>
        public string Flag { get; private set; }

        private void Load()
        {
            var flag = BitConverter.ToUInt32(RawData, (int) (ContentPosition + 28));
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

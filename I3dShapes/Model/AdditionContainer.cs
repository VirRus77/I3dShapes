using System;
using System.IO;

namespace I3dShapes.Model
{
    public class AdditionContainer
    {
        public AdditionContainer(BinaryReader reader)
        {
            Load(reader);
        }

        public uint Type { get; private set; }

        public byte[] RawData { get; private set; }

        private void Load(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            var size = reader.ReadUInt32();
            if (size > Int32.MaxValue)
            {
                throw new Exception("size > Int32.MaxValue");
            }

            RawData = reader.ReadBytes((int)size);
        }
    }
}

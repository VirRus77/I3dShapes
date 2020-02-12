using System.IO;
using I3dShapes.Exceptions;
using I3dShapes.Model;

namespace I3dShapes.Tests.Model
{
    public class FakeNamedObject : NamedShapeObject
    {
        public FakeNamedObject(uint rawType, BinaryReader reader)
            : base(ShapeType.Unknown)
        {
            RawType = rawType;
            Load(reader);
        }

        public uint RawType { get; }

        public byte[] RawData { get; private set; }

        private void Load(BinaryReader reader)
        {
            try
            {
                base.Load(reader);
            }
            catch (UnknownFormatShapeException ex)
            {
                throw;
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            RawData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
        }
    }
}

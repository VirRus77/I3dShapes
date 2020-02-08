using System.IO;
using System.Text;
using I3dShapes.Tools.Extensions;

namespace I3dShapes.Model
{
    public abstract class NamedShapeObject : ShapeObject, INamedShapeObject
    {
        protected NamedShapeObject(ShapeType type)
            : base(type)
        {
        }

        public ShapeType Type { get; }

        public string Name { get; private set; }

        protected void Load(BinaryReader reader)
        {
            var nameLength = reader.ReadInt32();
            Name = Encoding.ASCII.GetString(reader.ReadBytes(nameLength));
            reader.Align(4);
        }
    }
}

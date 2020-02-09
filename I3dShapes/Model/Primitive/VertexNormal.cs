using System.IO;

namespace I3dShapes.Model.Primitive
{
    public class VertexNormal
    {
        public VertexNormal(BinaryReader reader)
        {
            Load(reader);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private void Load(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }
}

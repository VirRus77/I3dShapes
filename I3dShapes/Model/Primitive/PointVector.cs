using System.IO;

namespace I3dShapes.Model.Primitive
{
    public class PointVector
    {
        public PointVector(BinaryReader reader)
        {
            Load(reader);
        }

        public float P1 { get; set; }
        public float P2 { get; set; }
        public float P3 { get; set; }

        private void Load(BinaryReader reader)
        {
            P1 = reader.ReadSingle();
            P2 = reader.ReadSingle();
            P3 = reader.ReadSingle();
        }
    }
}

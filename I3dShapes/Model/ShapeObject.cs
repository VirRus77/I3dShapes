namespace I3dShapes.Model
{
    public abstract class ShapeObject : IShapeObject
    {
        protected ShapeObject(ShapeType type)
        {
            Type = type;
        }

        public ShapeType Type { get; }
    }
}

namespace I3dShapes.Model
{
    public abstract class ShapeObject : IShapeObject
    {
        protected ShapeObject(ShapeType type)
        {
            Type = type;
        }

        /// <summary>
        /// Shape type
        /// </summary>
        public ShapeType Type { get; }
    }
}

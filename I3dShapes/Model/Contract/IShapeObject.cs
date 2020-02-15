namespace I3dShapes.Model.Contract
{
    /// <summary>
    /// Base on shape.
    /// </summary>
    public interface IShapeObject
    {
        /// <summary>
        /// Type shape object <see cref="ShapeType"/>
        /// </summary>
        ShapeType Type { get; }
    }
}

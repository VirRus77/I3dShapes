namespace I3dShapes.Model.Contract
{
    /// <summary>
    /// Representation named shape.
    /// </summary>
    public interface INamedShapeObject : IShapeObject
    {
        /// <summary>
        /// Shape Id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Shame name.
        /// </summary>
        string Name { get; }
    }
}

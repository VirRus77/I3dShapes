namespace I3dShapes.Model.Contract
{
    /// <summary>
    /// Binary representation named shape.
    /// </summary>
    public interface IRawNamedShapeObject : IRawShapeObject, INamedShapeObject
    {
        /// <summary>
        /// Position skip Id and Name (Align 4).
        /// </summary>
        long ContentPosition { get; }
    }
}

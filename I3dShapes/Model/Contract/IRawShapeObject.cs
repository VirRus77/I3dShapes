namespace I3dShapes.Model.Contract
{
    /// <summary>
    /// Binary representation shape.
    /// </summary>
    public interface IRawShapeObject : IShapeObject
    {
        /// <summary>
        /// Binary representation type.
        /// </summary>
        uint RawType { get; }

        /// <summary>
        /// Content.
        /// </summary>
        byte[] RawData { get; }
    }
}

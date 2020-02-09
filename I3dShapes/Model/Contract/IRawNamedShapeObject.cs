namespace I3dShapes.Model.Contract
{
    public interface IRawNamedShapeObject : IRawShapeObject, INamedShapeObject
    {
        /// <summary>
        /// Position skip Id and Name
        /// </summary>
        long ContentPosition { get; }
    }
}

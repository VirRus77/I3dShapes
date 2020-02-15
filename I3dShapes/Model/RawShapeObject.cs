using System.IO;
using I3dShapes.Model.Contract;
using I3dShapes.Tools.Extensions;

namespace I3dShapes.Model
{
    /// <inheritdoc cref="IRawShapeObject"/>
    public class RawShapeObject : ShapeObject, IRawShapeObject
    {
        /// <summary>
        /// Constructor <see cref="RawShapeObject"/>.
        /// </summary>
        /// <param name="rawType"><inheritdoc cref="IRawShapeObject.RawType"/></param>
        /// <param name="reader"><inheritdoc cref="IRawShapeObject.RawData"/></param>
        public RawShapeObject(uint rawType, BinaryReader reader)
            : base(ShapeType.Raw)
        {
            RawType = rawType;
            Load(reader);
        }

        /// <inheritdoc cref="IRawShapeObject.RawType"/>
        public uint RawType { get; }

        /// <inheritdoc cref="IRawShapeObject.RawData"/>
        public byte[] RawData { get; private set; }

        /// <summary>
        /// Load content.
        /// </summary>
        /// <param name="reader"><inheritdoc cref="BinaryReader"/></param>
        protected void Load(BinaryReader reader)
        {
            RawData = reader.ReadAll();
        }
    }
}

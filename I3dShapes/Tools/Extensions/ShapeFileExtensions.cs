using System.Collections.Generic;
using System.Linq;
using I3dShapes.Model;

namespace I3dShapes.Tools.Extensions
{
    /// <summary>
    /// Extensions by <see cref="ShapeFile"/>.
    /// </summary>
    public static class ShapeFileExtensions
    {
        /// <summary>
        /// Read all <see cref="Shape"/>.
        /// </summary>
        /// <param name="shapeFile"><inheritdoc cref="ShapeFile"/></param>
        /// <returns>Collection <see cref="Shape"/>.</returns>
        public static IEnumerable<Shape> ReadShapes(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.Shape)
                .OfType<Shape>();
        }

        /// <summary>
        /// Read all <see cref="Spline"/>.
        /// </summary>
        /// <param name="shapeFile"><inheritdoc cref="ShapeFile"/></param>
        /// <returns>Collection <see cref="Spline"/>.</returns>
        public static IEnumerable<Spline> ReadSplines(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.Spline)
                .OfType<Spline>();
        }

        /// <summary>
        /// Read all <see cref="NavMesh"/>.
        /// </summary>
        /// <param name="shapeFile"><inheritdoc cref="ShapeFile"/></param>
        /// <returns>Collection <see cref="NavMesh"/>.</returns>
        public static IEnumerable<NavMesh> ReadNavMesh(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.NavMesh)
                .OfType<NavMesh>();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using I3dShapes.Model;

namespace I3dShapes.Tools.Extensions
{
    public static class ShapeFileExtensions
    {
        public static IEnumerable<Shape> ReadShapes(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.Shape)
                .OfType<Shape>();
        }

        public static IEnumerable<Spline> ReadSplines(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.Spline)
                .OfType<Spline>();
        }

        public static IEnumerable<NavMesh> ReadNavMesh(this ShapeFile shapeFile)
        {
            return shapeFile.ReadKnowTypes(ShapeType.NavMesh)
                .OfType<NavMesh>();
        }
    }
}

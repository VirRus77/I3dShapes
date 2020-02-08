using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Tools.Extensions;
using I3dShapes.Container;
using I3dShapes.Model;
using I3dShapes.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace I3dShapes.Tests
{
    [TestClass]
    public class LibraryTests
    {
        private static readonly Dictionary<(FarmSimulatorVersion, string), Dictionary<ShapeType, int>> TestAnswers =
            new Dictionary<(FarmSimulatorVersion, string), Dictionary<ShapeType, int>>
            {
                {
                    (FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes"), new Dictionary<ShapeType, int>
                    {
                        { ShapeType.Type1, 1144 },
                        { ShapeType.Spline, 15 },
                        { ShapeType.Type3, 4 },
                    }
                },
                {
                    (FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes"), new Dictionary<ShapeType, int>
                    {
                        { ShapeType.Type1, 1013 },
                        { ShapeType.Spline, 12 },
                        { ShapeType.Type3, 4 },
                    }
                },
                {
                    (FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes"), new Dictionary<ShapeType, int>
                    {
                        { ShapeType.Type1, 1788 },
                        { ShapeType.Spline, 72 },
                        { ShapeType.Type3, 0 },
                    }
                },
                {
                    (FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes"), new Dictionary<ShapeType, int>
                    {
                        { ShapeType.Type1, 2240 },
                        { ShapeType.Spline, 107 },
                        { ShapeType.Type3, 0 },
                    }
                }
            };

        [TestMethod]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void LoadAllKnownTypes(FarmSimulatorVersion version, string shapeFileName)
        {
            var testAnswer = TestAnswers[(version, shapeFileName)];

            var gameMapPath = GamePaths.GetGameMapsPath(version);
            if (!Directory.Exists(gameMapPath))
            {
                var message = $"Game map path not found: \"{version}\".";
                Trace.WriteLine(message);
                Assert.Inconclusive(message);
            }

            var mapPath = Path.Combine(gameMapPath, shapeFileName);
            if (!File.Exists(mapPath))
            {
                var message = $"Map not found \"{version}\": \"{mapPath}\".";
                Trace.WriteLine(message);
                Assert.Inconclusive(message);
            }

            var container = new FileContainer(mapPath);
            var entities = container.GetEntities();
            // Load all known types
            var shapes = container.LoadKnowTypes(entities);

            var answer = shapes
                         .GroupBy(v => v.Type)
                         .Select(v => (v.Key, v.Count()))
                         .ToArray();
            answer
                .ForEach(v => Assert.AreEqual(testAnswer[v.Key], v.Item2));

            // OR Custom shape types
            shapes = container.LoadKnowTypes(
                entities,
                new[]
                {
                    ShapeType.Type1
                }
            );

            answer
                .ForEach(v => Assert.AreEqual(testAnswer[v.Key], v.Item2));
        }
    }
}

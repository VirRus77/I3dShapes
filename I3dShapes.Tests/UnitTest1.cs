using System.IO;
using System.Linq;
using Core.Tools.Extensions;
using I3dShapes.Container;
using I3dShapes.Tests.Model;
using I3dShapes.Tests.Tools;
using I3dShapes.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace I3dShapes.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly string OutputPath = @"G:\NickProd\Farming Simulator 19\Temp";

        //[TestMethod]
        /*
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        */
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void ExportShapeType1(in FarmSimulatorVersion version, in string mapName)
        {
            var outputPath = Path.Combine(OutputPath, version.ToString(), mapName);
            var mapPath = Path.Combine(GamePaths.GetGameMapsPath(version), mapName);
            if (!File.Exists(mapPath))
            {
                Assert.Inconclusive($"File map not found [{version}]: \"{mapName}\"");
            }

            var container = new FileContainer(mapPath);
            var entities = container.GetEntities();
            var shapes = container.LoadKnowTypes(entities);
            container
                .ReadRawData(entities.Where(v => v.Type == 1))
                .Select(
                    (entityRaw, i) =>
                    {
                        using var stream = new MemoryStream(entityRaw.RawData);
                        using var reader = new EndianBinaryReader(stream, container.Endian);
                        return new ReverseEngineeringNamedShape1Object(entityRaw.Entity.Type, reader);
                    }
                )
                .ForEach(
                    (v, i) =>
                    {
                        var output = Path.Combine(outputPath, v.RawType.ToString(), v.Flag);
                        if (!Directory.Exists(output))
                        {
                            Directory.CreateDirectory(output);
                        }

                        output = Path.Combine(output, $"{v.Flag}_[{v.Id}]_{FileTools.CleanFileName(v.Name)}.bin");
                        File.WriteAllBytes(output, v.RawData);
                    }
                );
        }

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes", 3)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes", 3)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes", 3)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes", 3)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes", 3)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes", 3)]
        public void ExportShapeType(in FarmSimulatorVersion version, in string mapName, int shapeType)
        {
            var outputPath = Path.Combine(OutputPath, version.ToString(), mapName);
            var mapPath = Path.Combine(GamePaths.GetGameMapsPath(version), mapName);
            if (!File.Exists(mapPath))
            {
                Assert.Inconclusive($"File map not found [{version}]: \"{mapName}\"");
            }

            var container = new FileContainer(mapPath);
            var entities = container.GetEntities();
            var shapes = container.LoadKnowTypes(entities);
            container
                .ReadRawData(entities.Where(v => v.Type == shapeType))
                .Select(
                    (entityRaw, i) =>
                    {
                        using var stream = new MemoryStream(entityRaw.RawData);
                        using var reader = new EndianBinaryReader(stream, container.Endian);
                        return new FakeNamedObject(entityRaw.Entity.Type, reader);
                    }
                )
                .ForEach(
                    (v, i) =>
                    {
                        var output = Path.Combine(outputPath, v.RawType.ToString());
                        if (!Directory.Exists(output))
                        {
                            Directory.CreateDirectory(output);
                        }

                        output = Path.Combine(output, $"[{v.Id}]_{FileTools.CleanFileName(v.Name)}.bin");
                        File.WriteAllBytes(output, v.RawData);
                    }
                );
        }

        //[TestMethod]
        [DataRow(@"G:\NickProd\Farming Simulator 19\Temp\FarmingSimulator2019\mapUS.i3d.shapes\Export\1346.i3d.shapes")]
        public void ExtractFile(string filePath)
        {
            var container = new FileContainer(filePath);
            var outDirectory = Path.GetDirectoryName(filePath);
            var entities = container.GetEntities();
            container
                .ReadRawData(entities)
                .Select(
                    (entityRaw, i) =>
                    {
                        using var stream = new MemoryStream(entityRaw.RawData);
                        using var reader = new EndianBinaryReader(stream, container.Endian);
                        return new FakeNamedObject(entityRaw.Entity.Type, reader);
                    }
                )
                .ForEach(
                    (v, i) =>
                    {
                        var output = Path.Combine(outDirectory, v.RawType.ToString());
                        if (!Directory.Exists(output))
                        {
                            Directory.CreateDirectory(output);
                        }

                        output = Path.Combine(output, $"[{v.Id}]_{FileTools.CleanFileName(v.Name)}.bin");
                        File.WriteAllBytes(output, v.RawData);
                    }
                );
        }
    }
}

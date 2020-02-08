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

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        public void ExportShapeType1(in FarmSimulatorVersion version, in string mapName)
        {
            var outputPath = Path.Combine(OutputPath, version.ToString(), mapName);
            var mapPath = Path.Combine(GamePaths.GetGameMapsPath(FarmSimulatorVersion.FarmingSimulator2017), mapName);
            if (!File.Exists(mapPath))
            {
                Assert.Inconclusive($"File map not found [{version}]: \"{mapName}\"");
            }

            var container = new FileContainer(mapPath);
            var entities = container.GetEntities();
            container
                .ReadRawData(entities)
                .Select(
                    entityRaw =>
                    {
                        using var stream = new MemoryStream(entityRaw.RawData);
                        using var reader = new EndianBinaryReader(stream, container.Endian);
                        return new FakeNamedObject(entityRaw.Entity.Type, reader);
                    }
                )
                .ForEach(
                    (v,i) =>
                    {
                        var output = Path.Combine(outputPath, v.RawType.ToString());
                        if (!Directory.Exists(output))
                        {
                            Directory.CreateDirectory(output);
                        }

                        output = Path.Combine(output, $"{i}_{CleanFileName(v.Name)}.bin");
                        File.WriteAllBytes(output, v.RawData);
                    }
                );
        }

        /// <summary>
        /// https://stackoverflow.com/a/7393722/2911165
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                       .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}

using System;
using System.IO;
using System.Linq;
using I3dShapes.Container;
using I3dShapes.Model;
using I3dShapes.Model.Contract;
using I3dShapes.Tests.Tools;
using I3dShapes.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq.Extensions;

namespace I3dShapes.Tests
{
    [TestClass]
    public class ReadAllShapesTest
    {
        /// <summary>
        /// Read ALL shapes in game directory.
        /// If 
        /// </summary>
        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2013)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019)]
        public void ReadAllShapes(FarmSimulatorVersion version)
        {
            var hasError = false;
            var gamePath = GamePaths.GetGamePath(version);
            if (gamePath == null || !Directory.Exists(gamePath))
            {
                Assert.Inconclusive($"Game path not found: {version}");
            }

            var shapeFiles = Directory.GetFiles(gamePath, $"*{GameConstants.SchapesFileExtension}", SearchOption.AllDirectories);
            shapeFiles
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForEach(
                    filePath =>
                    {
                        try
                        {
                            var container = new FileContainer(filePath);
                            var entities = container.GetEntities();
                            foreach (var valueTuple in container.ReadRawData(entities))
                            {
                                using (var stream = new MemoryStream(valueTuple.RawData))
                                {
                                    try
                                    {
                                        using (var reader = new EndianBinaryReader(stream, container.Endian, true))
                                        {
                                            switch (valueTuple.Entity.Type)
                                            {
                                                case 1:
                                                    var shape = new Shape(reader, container.Header.Version);
                                                    break;
                                                case 2:
                                                    var spline = new Spline(reader);
                                                    break;
                                                case 3:
                                                    var mesh = new NavMesh(reader);
                                                    break;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        hasError = true;
                                        stream.Seek(0, SeekOrigin.Begin);
                                        using (var reader = new EndianBinaryReader(stream, container.Endian))
                                        {
                                            SaveErrorShape(
                                                version,
                                                container.Header.Version,
                                                filePath,
                                                new RawNamedShapeObject(valueTuple.Entity.Type, reader, container.Endian)
                                            );
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            hasError = true;
                        }
                    }
                );

            Assert.IsFalse(hasError);
        }

        /// <summary>
        /// Save error parse shape in directory.
        /// </summary>
        /// <param name="version">Farming simulator version.</param>
        /// <param name="containerVersion">Container version.</param>
        /// <param name="shapeFileName">Shape file name.</param>
        /// <param name="rawShape"><inheritdoc cref="IRawNamedShapeObject"/></param>
        private static void SaveErrorShape(
            FarmSimulatorVersion version,
            short containerVersion,
            string shapeFileName,
            IRawNamedShapeObject rawShape
        )
        {
            var curentPath = Directory.GetCurrentDirectory();
            var outputPath = "Output";
            var outputDirectory = Path.Combine(
                curentPath,
                outputPath,
                version.ToString(),
                Path.GetFileName(shapeFileName)
                    .Replace(GameConstants.SchapesFileExtension, "")
            );
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var fileName = $"({containerVersion})[{rawShape.Id}]_[{rawShape.RawType}]_{FileTools.CleanFileName(rawShape.Name)}.bin";
            File.WriteAllBytes(Path.Combine(outputDirectory, fileName), rawShape.RawData);
        }
    }
}

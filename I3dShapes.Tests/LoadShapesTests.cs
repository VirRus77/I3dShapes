using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Tools.Extensions;
using I3dShapes.Container;
using I3dShapes.Model;
using I3dShapes.Tests.Model;
using I3dShapes.Tests.Tools;
using I3dShapes.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace I3dShapes.Tests
{
    [TestClass]
    public class LoadShapesTests
    {
        private static readonly FarmSimulatorVersion[] SupportVesion = {
            FarmSimulatorVersion.FarmingSimulator2017,
            FarmSimulatorVersion.FarmingSimulator2019
        };

        private static string OutputErrorShapes = @"G:\NickProd\Farming Simulator 19\Temp\Error";

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void LoadShapeType1Test(FarmSimulatorVersion version, string shapeFileName)
        {
            if (!SupportVesion.Contains(version))
            {
                var message = $"Currently not supported: \"{version}\".";
                Trace.WriteLine(message);
                Assert.Inconclusive(message);
            }
            LoadTypedShape(version, shapeFileName,1, reader => new ShapeType1(reader));
        }

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void LoadShapeType2Test(FarmSimulatorVersion version, string shapeFileName)
        {
            LoadTypedShape(version, shapeFileName,2, reader => new Spline(reader));
        }

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void LoadShapeType3Test(FarmSimulatorVersion version, string shapeFileName)
        {
            LoadTypedShape(version, shapeFileName,3, reader => new ShapeType3(reader));
        }

        public void LoadTypedShape<T>(FarmSimulatorVersion version, string shapeFileName, int rawType,Func<BinaryReader, T> loadShape)
        {
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

            // Clear directory by error shapes
            var errorOutputPath = Path.Combine(OutputErrorShapes, version.ToString(), shapeFileName);
            if (Directory.Exists(errorOutputPath))
            {
                Directory.Delete(errorOutputPath, true);
            }

            var fileContainer = new FileContainer(mapPath);
            var entities = fileContainer.GetEntities();
            var error = false;
            fileContainer.ReadRawData(entities)
                         .Where(v => v.Entity.Type == rawType)
                         .ForEach(
                             v =>
                             {
                                 using var stream = new MemoryStream(v.RawData);
                                 using var reader = new EndianBinaryReader(stream, fileContainer.Endian);
                                 try
                                 {
                                     var shape = loadShape(reader);
                                 }
                                 catch (Exception ex)
                                 {
                                     try
                                     {
                                         var shape = loadShape(reader);
                                     }
                                     catch (Exception e)
                                     {
                                     }

                                     error = true;
                                     Trace.WriteLine($"Error load shape.");
                                     var errorShape2 = new ReverseEngineeringNamedShape1Object(
                                         v.Entity.Type,
                                         v.RawData,
                                         fileContainer.Endian
                                     );
                                     Save(errorOutputPath, version, errorShape2, v.RawData);
                                 }
                             }
                         );

            if (error)
            {
                Assert.Fail();
            }
        }

        //[TestMethod]
        [DataRow(
            @"G:\NickProd\Farming Simulator 19\Temp\Error\FarmingSimulator2017\1\11000000_00000000_00000000_00000000\11000000_00000000_00000000_00000000_[755]_collisionShape.bin",
            Endian.Little
        )]
        public void DebugLoadFileShapeType1(string shapeFilePath, Endian endian)
        {
            using var stream = File.OpenRead(shapeFilePath);
            using var reader = new EndianBinaryReader(stream, endian);
            var shape = new ShapeType1(reader);
        }
        
        //[TestMethod]
        [DataRow(
            @"G:\NickProd\Farming Simulator 19\Temp\Error\FarmingSimulator2017\map01.i3d.shapes\2\00010001_11111110_10111001_11000010\00010001_11111110_10111001_11000010_[1114]_splineGeometry.bin",
            Endian.Little
        )]
        public void DebugLoadFileShapeType2(string shapeFilePath, Endian endian)
        {
            using var stream = File.OpenRead(shapeFilePath);
            using var reader = new EndianBinaryReader(stream, endian);
            var shape = new Spline(reader);
        }

        private void Save(
            string errorOutputPath,
            FarmSimulatorVersion version,
            ReverseEngineeringNamedShape1Object errorShape,
            byte[] rawData
        )
        {
            var outputDirectory = Path.Combine(errorOutputPath, errorShape.RawType.ToString(), errorShape.Flag);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var fileName = $"{errorShape.Flag}_[{errorShape.Id}]_{FileTools.CleanFileName(errorShape.Name)}.bin";
            var filePath = Path.Combine(outputDirectory, fileName);
            File.WriteAllBytes(filePath, rawData);
        }
    }
}

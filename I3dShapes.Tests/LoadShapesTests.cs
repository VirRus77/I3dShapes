namespace I3dShapes.Tests
{
#if REVERSE_ENGINEERING
    [TestClass]
    public class LoadShapesTests
    {
        private static string OutputErrorShapes = @"G:\NickProd\Farming Simulator 19\Temp\Error";

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void LoadShapeType1Test(FarmSimulatorVersion version, string shapeFileName)
        {
            //if (!SupportVesion.Contains(version))
            //{
            //    var message = $"Currently not supported: \"{version}\".";
            //    Trace.WriteLine(message);
            //    Assert.Inconclusive(message);
            //}
            LoadTypedShape(version, shapeFileName, 1, (reader, version) => new Shape(reader, version));
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
            LoadTypedShape(version, shapeFileName, 2, (reader, version) => new Spline(reader));
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
            LoadTypedShape(version, shapeFileName, 3, (reader, version) => new NavMesh(reader));
        }

        public void LoadTypedShape<T>(FarmSimulatorVersion version, string shapeFileName, int rawType, Func<BinaryReader, int, T> loadShape)
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
                        try
                        {
                            using var stream = new MemoryStream(v.RawData);
                            using var reader = new EndianBinaryReader(stream, fileContainer.Endian);
                            var shape = loadShape(reader, fileContainer.Header.Version);
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            using var stream = new MemoryStream(v.RawData);
                            using var reader = new EndianBinaryReader(stream, fileContainer.Endian);
                            Trace.WriteLine($"Error load shape.");
                            var errorShape2 = new ReverseEngineeringNamedShape1Object(
                                v.Entity.Type,
                                reader,
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

        [TestMethod]
        [DataRow(@"D:\SteamLibrary\steamapps\common\Farming Simulator 17\data\vehicles\tools\magsi\telehandlerLogFork.i3d.shapes")]
        public void DebugLoadFileShape(string shapeFilePath)
        {
            var output = @"G:\NickProd\Farming Simulator 19\Temp";
            var gameDirectory = GamePaths.GetGamePath(FarmSimulatorVersion.FarmingSimulator2017);
            var realtive = Path.GetRelativePath(gameDirectory, shapeFilePath);
            var shapeFile = new ShapeFile(shapeFilePath);
            var index = 0;
            try
            {
                shapeFile.ReadKnowTypes()
                    .ForEach((v, i) => index = i);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //shapeFile.ReadRawNamedShape()
            //         .ForEach(
            //             v =>
            //             {
            //                 var dirOutName = Path.Combine(output, realtive);
            //                 if (!Directory.Exists(dirOutName))
            //                 {
            //                     Directory.CreateDirectory(dirOutName);
            //                 }
            //                 File.WriteAllBytes(Path.Combine(dirOutName, $"[{v.Id}]_{FileTools.CleanFileName(v.Name)}.bin"), v.RawData);
            //             });
        }

        [TestMethod]
        [DataRow(
            @"G:\NickProd\Farming Simulator 19\Temp\Error\FarmingSimulator2015\map01.i3d.shapes\1\00000000_00001000_00000000_11111001\00000000_00001000_00000000_11111001_[469]_poplar40mLod0Shape.bin",
            (short)3
        )]
        public void DebugLoadFileShapeType1(string shapeFilePath, short version)
        {
            var endian = version < 4 ? Endian.Big : Endian.Little;
            using var stream = File.OpenRead(shapeFilePath);
            using var reader = new EndianBinaryReader(stream, endian);
            var shape = new Shape(reader, version);
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
#endif // REVERSE_ENGINEERING
}

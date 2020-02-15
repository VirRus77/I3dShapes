namespace I3dShapes.Tests
{
#if REVERSE_ENGINEERING
    [TestClass]
    public class ReverseEngineeringTests
    {
        private static readonly string OutputPath = @"G:\NickProd\Farming Simulator 19\Temp";


        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes")]
        public void ExportRawShapes(in FarmSimulatorVersion version, in string mapName)
        {
            var outputPath = Path.Combine(OutputPath, version.ToString(), mapName);
            var mapPath = Path.Combine(GamePaths.GetGameMapsPath(version), mapName);
            if (!File.Exists(mapPath))
            {
                Assert.Inconclusive($"File map not found [{version}]: \"{mapName}\"");
            }

            var container = new ShapeFile(mapPath);
            container.ReadRawNamedShape(4,5,6,7,8,9,10)
                .ForEach(v => Save(outputPath, v));
        }

        [TestMethod]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes")]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes")]
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

            outputPath = Path.Combine(outputPath, "1");
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            var shapeFile = new ShapeFile(mapPath);
            shapeFile.ReadRawNamedShape(1)
                .ForEach(v => SaveType1(outputPath, v, shapeFile.Container.Endian));
        }

        [TestMethod]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map01.i3d.shapes", 1)]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2015, "map02.i3d.shapes", 1)]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map01.i3d.shapes", 1)]
        //[DataRow(FarmSimulatorVersion.FarmingSimulator2017, "map02.i3d.shapes", 1)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapDE.i3d.shapes", 1)]
        [DataRow(FarmSimulatorVersion.FarmingSimulator2019, "mapUS.i3d.shapes", 1)]
        public void ExportShapeType(in FarmSimulatorVersion version, in string mapName, int shapeType)
        {
            var outputPath = Path.Combine(OutputPath, version.ToString(), mapName);
            var mapPath = Path.Combine(GamePaths.GetGameMapsPath(version), mapName);
            if (!File.Exists(mapPath))
            {
                Assert.Inconclusive($"File map not found [{version}]: \"{mapName}\"");
            }

            //var container = new FileContainer(mapPath);
            //var entities = container.GetEntities();
            //var shapes = container.LoadKnowTypes(entities);
            //container
            //    .ReadRawData(entities.Where(v => v.Type == shapeType))
            //    .Select(
            //        (entityRaw, i) =>
            //        {
            //            using var stream = new MemoryStream(entityRaw.RawData);
            //            using var reader = new EndianBinaryReader(stream, container.Endian);
            //            return new FakeNamedObject(entityRaw.Entity.Type, reader);
            //        }
            //    )
            //    .ForEach(
            //        (v, i) =>
            //        {
            //            var output = Path.Combine(outputPath, v.RawType.ToString());
            //            if (!Directory.Exists(output))
            //            {
            //                Directory.CreateDirectory(output);
            //            }

            //            output = Path.Combine(output, $"[{v.Id}]_{FileTools.CleanFileName(v.Name)}.bin");
            //            File.WriteAllBytes(output, v.RawData);
            //        }
            //    );
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

        private static void Save(string outputPath, IRawNamedShapeObject rawShapeObject)
        {
            outputPath = Path.Combine(outputPath, rawShapeObject.RawType.ToString());

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var fileName =
                $"[{rawShapeObject.Id}]_{FileTools.CleanFileName(rawShapeObject.Name)}.bin";
            File.WriteAllBytes(Path.Combine(outputPath, fileName), rawShapeObject.RawData);
        }

        private static void SaveType1(string outputPath, IRawNamedShapeObject rawShapeObject, Endian endian)
        {
            using var stream = new MemoryStream(rawShapeObject.RawData);
            using var reader = new EndianBinaryReader(stream, endian);
            var shape = new ReverseEngineeringNamedShape1Object(1, reader, endian);

            outputPath = Path.Combine(outputPath, shape.Flag);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var fileName =
                $"[{shape.Id}]_[{shape.Flag}]_{FileTools.CleanFileName(rawShapeObject.Name)}.bin";
            File.WriteAllBytes(Path.Combine(outputPath, fileName), rawShapeObject.RawData);
        }
    }
#endif // REVERSE_ENGINEERING
}

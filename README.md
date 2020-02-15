[![VirRus77.I3dShapes](https://img.shields.io/nuget/v/VirRus77.I3dShapes)](https://www.nuget.org/packages/VirRus77.I3dShapes)
# I3dShapes
Library used for extracting the binary .i3d.shapes files used by the GIANTS engine.
* (Based on https://github.com/Donkie/I3DShapesTool)

# Summary
1. Reverse engineering of shape 1, 2 and 3 type.

# Usage
For unloading parsed objects:
```C#
var shapeFile = new ShapeFile(shapeFilePath);

// Load all known types
var shapes = shapeFile.ReadKnowTypes();

// OR Custom shape types
shapes = container.ReadKnowTypes(ShapeType.Type1, ShapeType.Spline, ShapeType.NavMesh);

var shapesType1 = shapes.OfType<ShapeType1>().ToArray();
```

For unloading raw objects:
```C#
var shapeFile = new ShapeFile(shapeFilePath);

// Load all types
var shapes = shapeFile.ReadRawShape();

// OR Custom shape types
shapes = shapeFile.ReadRawShape(1, 2, 3);
```

For unloading named raw objects:
```C#
var shapeFile = new ShapeFile(shapeFilePath);

// Load all types
var shapes = shapeFile.ReadRawNamedShape();

// OR Custom shape types
shapes = shapeFile.ReadRawNamedShape(1, 2, 3);
```

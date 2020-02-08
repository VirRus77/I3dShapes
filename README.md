# I3dShapes
Library used for extracting the binary .i3d.shapes files used by the GIANTS engine.
* (Based on https://github.com/Donkie/I3DShapesTool)

# Summary
1. Type 1 shape shaping reverse engineering (for FS 2017 and FS 2019).
2. Reverse engineering of shape 2 type.

# Usage
```C#
var container = new FileContainer(shapeFilePath);
var entities = container.GetEntities();
// Load all known types
var shapes = container.LoadKnowTypes(entities);
// OR Custom shape types
shapes = container.LoadKnowTypes(entities, new[]{ ShapeType.Type1 });

var shapesType1 = shapes.OfType<ShapeType1>.ToArray();
```

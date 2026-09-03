# Orbit Navigation Example

This example shows how to setup and use orbit navigation scheme.

- The `GenericPlayer` and `EventSystem` prefabs are added to the scene.

- The `GenericPlayer/Generic Navigation/Type` is set to *Orbit*.

- The `GenericPlayer/Generic Navigation/Orbit Navigation Setings/Center`
  transform is set to the cylinder. Note that if no center was set, then
  the middle of the set would have been used instead.

- A meaningful radius can be set depending on both the display system
  physical size and the size of the objects to visualize.

⚠ Note that this navigation scheme doesn't *zoom* on the objects. It
*moves the camera* closer to the center. The perceived object size stays
the same.

In order to change the size of of the objects, the
`Runtime/Examples/OrbitNavigation/SceneScaleExample.cs` script is added
to a root object containing all objects to visualize. This script reacts
to getReal3D inputs and changes the overall scene scale.

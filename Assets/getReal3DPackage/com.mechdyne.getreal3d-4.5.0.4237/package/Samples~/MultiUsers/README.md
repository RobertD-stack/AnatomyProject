# Multi Users Example

This example shows how to have multiple users within a single getReal3D display
system.

Two `GenericPlayer` prefabs have been added to the scene. On the second one,
the following modifications have been made:

- The `UserId` has been set to 1 in the `[getReal3D]/GetRealUser` script. It
tells getReal3D to use the correct user for the cameras, the head and the wand.

- The axes and buttons binding names have been changed (a 2 suffix was added) on
  each of the bindings in the `[getReal3D]/GetReal3DPlayerInputs` script.

In order to test this scene, the `Two users configuration` should be set in the
getReal3D settings menu (`getReal3D -> Advanced -> Settings`). This
configuration defines 2 users with different tracking bound to the head and the
wand. Also it defines a bunch of new axis and button binding to match the
bindings modified above.

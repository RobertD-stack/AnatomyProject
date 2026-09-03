# Chaperone Grid Example

This example shows how to create a simple chaperone grid that is displayed when
the hand or the head of the user is moved close to a screen.

The `ChaperonePrefab` contains a mesh that is showing a grid for a single screen.

The `ChaperoneManagerPrefab` prefab is added to a GenericPlayer. There are two
scripts attached to this prefab:

- `CreateScreens` script creates as many `ChaperonePrefab` as there are screens
  (which ultimately depends on the getReal3D configuration file).

- `ChaperoneManager` script computes the distance between the head (and/or wand)
  of the user and any of the screen. If that distance is smaller than a
  threshold, all the chaperone grids are displayed.

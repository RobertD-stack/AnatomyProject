# Simple Example

This example shows the simplest way to integrate getReal3D into a Unity project.

The scene consists of a simple plane and a light. Two getReal3D prefabs assets
have been added:

- The `GenericPlayer` prefab is the main part. It handles navigation of the user,
  displaying the correct cameras according to the getReal3D configuration file
  and a basic 3D menu that can be used to change some settings.

- The `EventSystem` prefab is used to handle 3D interaction between the user hand
  and the 3D menu attached to the `GenericPlayer`.

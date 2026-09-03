# Custom RPC Example

This example shows how to use custom serialization for Remote Procedure Calls.

In this example, a 2D UI is displayed on the master node only. Since
interaction only happens on the master, RPCs are required to handle sending the
actions on the whole cluster.

- Add both a `GenericPlayer` and `EventSystem` prefabs to the scene.

- Add a sphere in front of the player. Hit play and verify that the sphere is
  visible.

- In order to not mess up with the 2D UI, disable the following scripts from
  the GenericPlayer:

  - `Generic Show Menu`
  - `Menu Settings`
  - `Get Real VR Settings UI`

- In order to use the master keyboard and mouse on the 2D UI, disable the
  `Generic Wand Event Module` from the `EventSystem` and add a
  `Standalone Input Module` in it.

- Create a simple button UI by right clicking on an empty space of the Hierachy
  then choose `UI → Button`.

- Verify that the `Canvas` render mode is set to `Screen Space - Overlay`.

- Add the `CustomRpcExample` component to the root object of the `Canvas`.

- Assign the sphere to the `CustomRpcExample` targetRenderer field using drag and
  drop.

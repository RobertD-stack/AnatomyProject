# Show Inputs Example

This example lists all button and axis inputs from getReal3D and show their
values in a 3D UI located at screen space.

The UI is defined in the ShowInputs prefab. It is added in the
`GenericPlayer/CreateScreens` script so that each getReal3D screen has its own
UI. Within the UI, one ShowInputsButton prefab is created per button and one
ShowInputsAxisSlider prefab is created per getReal3D axis.

The script located in `Runtime/Examples/ShowInputs/ShowInputs.cs` builds and
updates the UI according to the buttons and axes state.

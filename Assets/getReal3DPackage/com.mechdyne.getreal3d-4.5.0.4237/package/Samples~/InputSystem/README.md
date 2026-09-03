# Input System Example

This example shows how to use [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5) with getReal3D.

The Unity Input System should be enabled in the Project Settings as specified in the documentation above. When Unity Input System is enabled, getReal3D creates a `getReal3D Controller` device exposing the inputs that are defined within the getReal3D Input Editor. It is then possible to bind getReal3D inputs as any other Unity native inputs.

In this example:
- An Input System [Action Asset](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/Workflow-ActionsAsset.html) is used to bind getReal3D inputs to Actions.
- Within that Action Asset, three actions are defined: Add Cubes, Clear Cubes and Change Cube Scale.
- Those actions have bindings to the `getReal3D Device` that can be found within the list in the `Other -> getReal3D` category.
- A script that acts on those actions and creates cubes falling from the hand position, while the Clear Cubes action destroy those. The script is located in the `com.mechdyne.getreal3d\InputSystem\Examples` folder. Note that this script doesn't contain any getReal3D specific code.
- This script uses Action Asset via the `Generate C# class feature` method as described in the Action Asset documentation.

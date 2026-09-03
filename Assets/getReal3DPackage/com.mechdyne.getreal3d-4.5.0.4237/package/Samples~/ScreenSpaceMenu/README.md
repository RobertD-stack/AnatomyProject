# Screen Space Menu Example

This example shows how to create a menu that is showing up at a specific screen
location. The menu shows some text, a previous and a next button. When
activated using the wand, the menu is moved to an another screen.

The menu is created in the ScreenSpaceMenu prefab and should be added to the
GenericPlayer as in the example scene.

The main logic is located in the
`Runtime\Examples\ScreenSpaceMenu\ScreenSpaceMenu.cs` file. This script relies
on the `getReal3D.CreateScreens` script to create game object corresponding to
the screens. The menu is then added to the first of those screens. When one of
the buttons is clicked, the menu is re-parented to an another of those screen
gameobject, and its transform is reset to match the location of the screen.

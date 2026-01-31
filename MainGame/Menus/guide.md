# Menu & Scene Transition System:

System for loading / transitioning between screens.
This includes menus as well as the main game area.

## How it works:

**TransitionToScene(Scenes S)** takes an enum value S for which scene to transition to.
This method handles loading & managing different screens.
This should be the only method that needs to be called.
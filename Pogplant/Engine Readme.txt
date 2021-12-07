By default, engine is in editor mode with all our debug objects.
To change to game build, remove #define PPD_EDITOR_BUILD from application.h

Pogplant.sln is the main project, and PogplantDriver project should be the startup exe

ScriptingBuild.sln is the project for the C# scripts, and can be added/built while the editor is still running.

To play the game in our editor, either remove #define PPD_EDITOR_BUILD, or do
file->open..-> and open Level01_M3_Blockout.json in Resources/Scenes, then press play.


Controls for engine side:

Keyboard

Move - WASD
Fire - Spacebar/Left mouse button

Xbox Controller:

Move - Left thumbstick
Fire - RT

Dualshock Controller

Move - Left thumbstick
Fire - R2
By default, engine is in editor mode with all our debug objects.
To change to game build, remove #define PPD_EDITOR_BUILD from application.h

Pogplant.sln is the main project, and PogplantDriver project should be the startup exe

ScriptingBuild.sln is the project for the C# scripts, and can be added/built while the editor is still running.

To play the game in our editor, either remove #define PPD_EDITOR_BUILD, or do
file->open..-> and open Level01.json in Resources/Scenes, then press play.

Editor: (Uncomment out #define PPD_EDITOR_BUILD on Windows.h line 3 under Pogplant > Source Files > Window to access editor)
	
	WASD 	- Movement
	Q/R 	- Up/Down
	Z/C 	- Roll Camera
	X 	- Teleport to selected entity
	2 	- Gizmo Move
	3 	- Gizmo Rotate
	4 	- Gizmo Scale
	Scene Hierarchy   	- Display all entities in scene
	Inspector  	  	- Inspects the components of selected entity
	Debugger 	  	- Toggle Collision Box rendering
	Globals 	  	- Adjust camera settings
	Debug 		  	- Shows debug information/printout
	Director 	  	- View/used to add or edit prefabs
	Prefab_Loaded 	  	- Display prefabs that are loaded into the scene(For debugging)
	Collision Layer Editor	- View/edit collision layer interactions, debug information/settings for collision
	Input Editor		- View/edit controls for keyboard/controller


Controls for game side:

Keyboard

Move - WASD
Fire - Spacebar/Left mouse button
Fire Nuke - Right mouse button

Xbox Controller:

Move - Left thumbstick
Fire - RT
Fire Nuke - B button

Dualshock Controller

Move - Left thumbstick
Fire - R2
Fire Nuke - Circle
# RoguelikeEcsBachelor
This repository contains builds of the game and the source code.

# Builds
Local builds of the game are in the [Builds](https://github.com/Chillu1/RoguelikeEcsBachelor/tree/master/Builds) directory, split by platform.

To run the standalone builds, simply run the windows, or linux executable.

To run the WebGL build locally, a simple http server is needed. 
With commands like: 

"http-server -p 8000"

or

"python -m http.server"

# Source Code
All ECS logic is inside of [Source/Assets/Scripts/Core/Ecs](https://github.com/Chillu1/RoguelikeEcsBachelor/tree/master/Source/Assets/Scripts/Core/Ecs) directory. 
The rest of the scripts, like benchmark scripts and non-ECS scripts are in the 
[Source/Assets/Scripts/](https://github.com/Chillu1/RoguelikeEcsBachelor/tree/master/Source/Assets/Scripts) directory.

# Online Builds
Online builds of the game are available at: 
* [Itch.io](https://chillu.itch.io/pixelwizardarena)
* [Github](https://chillu1.github.io/RoguelikeEcsWeb)

# Unity Project
To open the Unity project of the game:
* Install Unity Hub from https://unity.com/download.
* Install the Unity version 2021.3.16f1 from the Unity Hub.
* Inside the Unity Hub click "Open", and then select the "Source" folder inside
the repository.
* Open the project in Unity Hub.
* To start a game, select a "MainMenu" scene in /Assets/Scenes in project tab (bottom
left corner) in Unity Editor. Then click the start symbol above the scene window.

The game was build in Unity 2021.3.16f1, and should be build on that version as well

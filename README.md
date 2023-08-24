![alt text](https://github.com/itsEvil/UnityMapEditor/blob/31cd40c7b279bd27d958a150f28ba5bb602e84b8/logo.png?raw=true)

# UnityMapEditor
Public version of my map editor

# Requirements

Unity.
Unity Editor version 2022.3.0f1 or Later

# How to use it

1. Run the project or build it
2. Input width and height into boxes or load a previous map. (Can load wmap, .jm and .map)
3. Profit

# Info

This editor is VERY barebones so assets such as Tiles and Objects are not included but its very simple to add assets in.

This editor uses sight radius if the map size is greater then 256x256.
This is done to improve performance.

# Known Bugs

1. When loading .wmap or .jm maps placing new tiles/objects doesnt work or bugs out? 
### Work around is to load the map then save it as .map and load it again then it seems to work idek... 

2. Had issues saving to the .jm standard private servers use so I made my own standard .map which is around 4x larger filesize, maybe someone can help me figure this one out :/

You can read .map files if you just copy the contents of JSONMap ctor to your server source

# How do I use my assets?

You have to do all of these steps for your assets to work!
1. Add the sprite sheets into the "Resources / SpriteSheets" folder
2. In the SpriteSheets folder there is a .xml file, This xml file defines each sprite sheet so add your sprite sheets in here!
3. Add your XMLs into the "Resources / Xmls" folder.
4. Profit?
 

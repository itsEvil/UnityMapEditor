using Models.Static;
using Models;
using System.Collections.Generic;
public class Map
{
    public static Map LoadedMap;

    public MapTile[,] Tiles;
    public int Width;
    public int Height;
    public Dictionary<Region, List<IntPoint>> Regions;
}


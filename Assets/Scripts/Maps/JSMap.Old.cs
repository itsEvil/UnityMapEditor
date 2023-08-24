using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Models.Static;
using RotMG.Networking;
using Models;
using Ionic.Zlib;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public partial class JSMap : Map
{
    public JSMap()
    {

    }
    public static JSMap LoadOldJson(string data)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        JSMap map = new JSMap();

        var json = JsonConvert.DeserializeObject<Encode_json_dat>(data);
        var compressed = Convert.FromBase64String(json.data);
        var decoded = ZlibStream.UncompressBuffer(compressed);

        var dict = new Dictionary<ushort, MapTile>();
        var tiles = new MapTile[json.width, json.height];

        for (var i = 0; i < json.dict.Length; i++)
        {
            loc o = json.dict[i];

            var tile = ScriptableObject.CreateInstance<MapTile>();

            tile.GroundType = o.ground == null ? (ushort)255 : AssetLibrary.Id2TileDesc[o.ground].Type;
            tile.ObjectType = o.objs == null ? (ushort)0 : AssetLibrary.Id2ObjectDesc[o.objs[0].id].Type;
            tile.Key = o.objs?[0].name;
            tile.Region = o.regions == null ? Region.None : (Region)Enum.Parse(typeof(Region), o.regions[0].id.Replace(" ", ""));


            dict[(ushort)i] = tile;
        }

        using (var rdr = new PacketReader(new MemoryStream(decoded)))
        {
            for (var y = 0; y < json.height; y++)
                for (var x = 0; x < json.width; x++)
                {
                    ushort val = (ushort)rdr.ReadInt16();

                    //Debug.Log($"Reading position {val}");

                    tiles[x, y] = dict[val];
                }
        }

        //Add composite under cave walls
        for (var x = 0; x < json.width; x++)
        {
            for (var y = 0; y < json.height; y++)
            {
                if (tiles[x, y].ObjectType != 0)
                {
                    var desc = AssetLibrary.Type2ObjectDesc[tiles[x, y].ObjectType];
                    if ((desc.Class == ObjectType.CaveWall || desc.Class == ObjectType.ConnectedWall) && tiles[x, y].GroundType == 255)
                    {
                        tiles[x, y].GroundType = 0xfd;
                    }
                }
            }
        }

        map.Tiles = tiles;
        map.Width = json.width;
        map.Height = json.height;

        map.Regions = new Dictionary<Region, List<IntPoint>>();
        for (var x = 0; x < map.Width; x++)
            for (var y = 0; y < map.Height; y++)
            {
                var tile = map.Tiles[x, y];
                if (!map.Regions.ContainsKey(tile.Region))
                    map.Regions[tile.Region] = new List<IntPoint>();
                map.Regions[tile.Region].Add(new IntPoint(x, y));
            }

        stopwatch.Stop();
        Debug.Log($"Loading old JS took {stopwatch.ElapsedMilliseconds}ms");
        return map;
    }
}
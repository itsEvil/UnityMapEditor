using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;
using Models.Static;
using Models;
using System.Text;
using Ionic.Zlib;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public partial class JSMap : Map
{
    public JSMap(string data)
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        var json = JsonConvert.DeserializeObject<mapData>(data);
        var tiles = new MapTile[json.width, json.height];

        Regions = new();

        foreach (var tile in json.entires.tiles)
        {

            var decoded = Convert.FromBase64String(tile.data);

            var uncompressed = ZlibStream.UncompressString(decoded);

            string[] string_positions = uncompressed.Split(",");

            foreach(var pos in string_positions)
            {
                string[] values = pos.Split(":");

                if (values.Length == 0)
                    continue;

                if (!int.TryParse(values[0], out int x))
                {
                    continue;
                }

                if (!int.TryParse(values[1], out int y))
                {
                    continue;
                }

                ushort type = 0xff;

                if (AssetLibrary.Type2TileDesc.TryGetValue((int)tile.type, out var desc))
                {
                    type = desc.Type;
                }

                var item = tiles[x, y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.GroundType = type;
                tiles[x, y] = item;
                
            }
            /*if (tile.positions == null)
                continue;

            ushort type = 0xff;
            if (AssetLibrary.Type2TileDesc.TryGetValue((int)tile.type, out var desc))
            {
                type = desc.Type;
            }
            foreach (var pos in tile.positions)
            {
                var item = tiles[pos.X, pos.Y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.GroundType = type;
                tiles[pos.X, pos.Y] = item;
            }*/
        }
        foreach (var obj in json.entires.objects)
        {
            var decoded = Convert.FromBase64String(obj.data);

            var uncompressed = ZlibStream.UncompressString(decoded);

            string[] string_positions = uncompressed.Split(",");

            foreach (var pos in string_positions)
            {
                string[] values = pos.Split(":");

                if (values.Length == 0)
                    continue;

                if (!int.TryParse(values[0], out int x))
                {
                    continue;
                }

                if (!int.TryParse(values[1], out int y))
                {
                    continue;
                }

                int type = 0xff;
                if (AssetLibrary.Type2ObjectDesc.TryGetValue(obj.type, out var desc))
                {
                    type = desc.Type;
                }

                var item = tiles[x, y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.ObjectType = type;
                tiles[x, y] = item;
            }

            /*if (obj.positions == null)
                continue;

            ushort type = 0xff;
            if (AssetLibrary.Type2ObjectDesc.TryGetValue((int)obj.type, out var desc))
            {
                type = desc.Type;
            }
            foreach (var pos in obj.positions)
            {
                var item = tiles[pos.X, pos.Y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.ObjectType = type;
                tiles[pos.X, pos.Y] = item;
            }*/
        }
        foreach (var reg in json.entires.regions)
        {
            var decoded = Convert.FromBase64String(reg.data);

            var uncompressed = ZlibStream.UncompressString(decoded);

            string[] string_positions = uncompressed.Split(",");

            foreach (var pos in string_positions)
            {
                string[] values = pos.Split(":");

                if (values.Length == 0)
                    continue;

                if (!int.TryParse(values[0], out int x))
                {
                    continue;
                }

                if (!int.TryParse(values[1], out int y))
                {
                    continue;
                }

                Region region = AssetLibrary.Type2RegionDesc[(ushort)reg.type].RegionValue;
                
                var item = tiles[x, y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.Region = region;
                tiles[x, y] = item;

                //Init regions
                if (!Regions.TryGetValue(item.Region, out List<IntPoint> positions))
                {
                    positions = new List<IntPoint>();
                }
                positions.Add(new IntPoint(x, y));
                Regions[item.Region] = positions;
            }

            /*if (reg.positions == null)
                continue;

            Region region = AssetLibrary.Type2RegionDesc[(ushort)reg.type].RegionValue;
            foreach (var pos in reg.positions)
            {
                var item = tiles[pos.X, pos.Y];
                item ??= ScriptableObject.CreateInstance<MapTile>();
                item.Region = region;
                tiles[pos.X, pos.Y] = item;

                //Init regions
                if (!Regions.TryGetValue(item.Region, out List<IntPoint> values))
                {
                    values = new List<IntPoint>();
                }
                values.Add(new IntPoint(pos.X, pos.Y));
                Regions[item.Region] = values;
            }*/
        }

        Tiles = tiles;
        Width = json.width;
        Height = json.height;

        stopwatch.Stop();
        Debug.Log($"Loading .map took {stopwatch.ElapsedMilliseconds}ms");
    }
    public static string SerializeMap(MapTile[,] tiles, Bounds bounds)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        string data = "";

        if(bounds.min.x == 0 && bounds.min.y == 0 && bounds.max.x == 0 && bounds.max.y == 0)
        {
            Debug.LogError("Failed to get bounds of the map");
            return "FAILED TO SAVE";
        }

        mapData mapData = new mapData();
        entires entires = new entires();

        Dictionary<int, Entry> tileDict = new();
        Dictionary<int, Entry> objDict = new();
        Dictionary<int, Entry> regDict = new();

        int yi = (int)bounds.min.y;
        int xi = (int)bounds.min.x;

        int myi = (int)bounds.max.y;
        int mxi = (int)bounds.max.x;

        mapData.height = (int)bounds.size.y;
        mapData.width = (int)bounds.size.x;

        Debug.Log($"Bounds: x:{bounds.min.x} y:{bounds.min.y} height:{bounds.size.y} width:{bounds.size.x} maxX:{bounds.max.x} maxY:{bounds.max.y}");

        for (int y = 0; y < mapData.height; y++)
        {
            for (int x = 0; x < mapData.width; x++)
            {
                var tile = tiles[x + xi, y + yi];

                if(tile.GroundType != 0)
                {
                    if(tileDict.TryGetValue(tile.GroundType, out Entry val))
                    {
                        val.Positions.Add(new Position(x, y));
                    }
                    else
                    {
                        tileDict[tile.GroundType] = new Entry()
                        {
                            Type = tile.GroundType,
                            Positions = new List<Position>()
                            {
                                new Position(x,y)
                            }
                        };
                    }
                }
                if(tile.ObjectType != 0)
                {
                    if (objDict.TryGetValue(tile.ObjectType, out Entry val))
                    {
                        val.Positions.Add(new Position(x,y));
                    }
                    else
                    {
                        objDict[tile.ObjectType] = new Entry() 
                        { 
                            Type =  tile.ObjectType, 
                            Positions = new List<Position>() 
                            {
                                new Position(x,y) 
                            } 
                        };
                    }
                }
                if(tile.Region != Region.None)
                {
                    if (regDict.TryGetValue((int)tile.Region, out Entry val))
                    {
                        val.Positions.Add(new Position(x, y));
                    }
                    else
                    {
                        regDict[(int)tile.Region] = new Entry()
                        {
                            Type = (int)tile.Region,
                            Positions = new List<Position>()
                            {
                                new Position(x,y)
                            }
                        };
                    }
                }
            }
        }

        int index = 0;
        entry[] tileEntries = new entry[tileDict.Count];
        foreach(Entry ent in tileDict.Values)
        {
            if (ent.Type == 0)
                continue;

            int posIndex = 0;

            StringBuilder sb = new StringBuilder();
            foreach(var position in ent.Positions)
            {
                if(posIndex++ != ent.Positions.Count)
                {
                    sb.Append(position.ToString() + ",");
                }
                else
                    sb.Append(position.ToString());
            }

            var compressed = ZlibStream.CompressString(sb.ToString());

            entry entry = new entry()
            {
                type = ent.Type,
                data = ByteArrayToBase64Encode(compressed)
                //StringToBase64Encode(sb.ToString())
                //positions = ent.Positions.ToArray()
            };

            tileEntries[index++] = entry;
        }
        index = 0;
        entry[] objEntries = new entry[objDict.Count];
        foreach (Entry ent in objDict.Values)
        {
            if (ent.Type == 0)
                continue;


            int posIndex = 0;

            StringBuilder sb = new StringBuilder();
            foreach (var position in ent.Positions)
            {
                if (posIndex++ != ent.Positions.Count)
                {
                    sb.Append(position.ToString() + ",");
                }
                else
                    sb.Append(position.ToString());
            }

            var compressed = ZlibStream.CompressString(sb.ToString());

            entry entry = new entry()
            {
                type = ent.Type,
                data = ByteArrayToBase64Encode(compressed)
                //StringToBase64Encode(sb.ToString())
                //positions = ent.Positions.ToArray()
            };

            objEntries[index++] = entry;
        }
        index = 0;
        entry[] regEntries = new entry[regDict.Count];
        foreach (Entry ent in regDict.Values)
        {
            if (ent.Type == 0)
                continue;

            int posIndex = 0;

            StringBuilder sb = new StringBuilder();
            foreach (var position in ent.Positions)
            {
                if (posIndex++ != ent.Positions.Count)
                {
                    sb.Append(position.ToString() + ",");
                }
                else
                    sb.Append(position.ToString());
            }

            var compressed = ZlibStream.CompressString(sb.ToString());

            entry entry = new entry()
            {
                type = ent.Type,
                data = ByteArrayToBase64Encode(compressed)
                //StringToBase64Encode(sb.ToString())
                //positions = ent.Positions.ToArray()
            };

            regEntries[index++] = entry;
        }

        entires.tiles = tileEntries;
        entires.objects = objEntries;
        entires.regions = regEntries;

        mapData.entires = entires;

        data = JsonConvert.SerializeObject(mapData, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        stopwatch.Stop();
       
        Debug.Log($"Saving map took {stopwatch.ElapsedMilliseconds}ms");
        return data;
    }
    public static string ByteArrayToBase64Encode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }
}
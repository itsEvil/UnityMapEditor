using Ionic.Zlib;
using Models.Static;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public sealed class WMap : Map
{
    public WMap(byte[] mapData)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var memStream = new MemoryStream(mapData);
        var version = memStream.ReadByte();
        if (version < 0 || version > 2)
            throw new NotSupportedException("WMap version " + version);

        using (var rdr = new BinaryReader(new ZlibStream(memStream, CompressionMode.Decompress)))
        {
            var tiles = new List<MapTile>();
            var tileCount = rdr.ReadInt16();
            for (var i = 0; i < tileCount; i++)
            {
                var tile = ScriptableObject.CreateInstance<MapTile>();
                tile.GroundType = rdr.ReadUInt16();
                var obj = rdr.ReadString();
                tile.ObjectType = 0;
                if (AssetLibrary.Id2ObjectDesc.ContainsKey(obj))
                    tile.ObjectType = AssetLibrary.Id2ObjectDesc[obj].Type;
#if DEBUG
                else if (!string.IsNullOrEmpty(obj))
                    UnityEngine.Debug.Log($"Object: {obj} not found.");
#endif

                tile.Key = rdr.ReadString();
                _ = rdr.ReadByte();
                tile.Region = (Region)rdr.ReadByte();
                if (version == 1)
                    _ = rdr.ReadByte();
                tiles.Add(tile);
            }

            Width = rdr.ReadInt32();
            Height = rdr.ReadInt32();
            Tiles = new MapTile[Width, Height];

            Regions = new Dictionary<Region, List<IntPoint>>();
            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    var tile = tiles[rdr.ReadInt16()];
                    if (version == 2)
                        _ = rdr.ReadByte();

                    if (!Regions.ContainsKey(tile.Region))
                        Regions[tile.Region] = new List<IntPoint>();
                    Regions[tile.Region].Add(new IntPoint(x, y));

                    Tiles[x, y] = tile;
                }

            //Add composite under cave walls
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var tile = Tiles[x, y];

                    if (tile.ObjectType != 0)
                    {
                        if(!AssetLibrary.Type2ObjectDesc.TryGetValue(tile.ObjectType, out var desc))
                        {
                            continue;
                        }

                        if ((desc.Class == ObjectType.CaveWall || desc.Class == ObjectType.ConnectedWall) && tile.GroundType == 255)
                        {
                            tile.GroundType = 0xfd;
                        }
                    }
                }
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Wmap loading took {stopwatch.Elapsed}");
    }
}


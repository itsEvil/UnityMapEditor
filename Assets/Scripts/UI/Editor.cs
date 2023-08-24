using Models.Static;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public partial class Editor : MonoBehaviour
{
    private MapTile[,] Tiles;
    private int _width, _height;
    public void LoadMap(Map map)
    {
        t_Tiles.ClearAllTiles();
        t_Objects.ClearAllTiles();
        t_Regions.ClearAllTiles();

        Tiles = new MapTile[map.Width, map.Height];
        LoadedTiles = new int[map.Width, map.Height];
        Tiles = map.Tiles;
        
        _width = map.Width;
        _height = map.Height;

        if(_width < 256 && _height < 256)
        {
            RenderMap();
        }

        _camera.transform.SetPositionAndRotation(new Vector3(_width / 2, _height / 2, -10), Quaternion.identity); //Middle

        t_Tiles.RefreshAllTiles();
        t_Objects.RefreshAllTiles();
        t_Regions.RefreshAllTiles();

        _lineRenderer.positionCount = 4;
        _lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        _lineRenderer.SetPosition(1, new Vector3(_width, 0, 0));
        _lineRenderer.SetPosition(2, new Vector3(_width, _height, 0));
        _lineRenderer.SetPosition(3, new Vector3(0, _height, 0));
    }

    private void RenderMap()
    {
        for(int x =0; x < _width; x++)
        {
            for(int y =0; y < _height; y++)
            {
                var tile = Tiles[x, y];

                var pos = new Vector3Int(x, y);

                if (tile.GroundType != 255)
                {
                    var newTile = ScriptableObject.CreateInstance<MapTile>();
                    newTile.GroundType = tile.GroundType;
                    t_Tiles.SetTile(pos, newTile);
                    newTile.Render(Models.Static.RenderType.Tile);
                    t_Tiles.RefreshTile(pos);
                }

                if (tile.ObjectType != 0)
                {
                    var newTile = ScriptableObject.CreateInstance<MapTile>();
                    newTile.ObjectType = tile.ObjectType;
                    t_Objects.SetTile(pos, newTile);
                    newTile.Render(Models.Static.RenderType.Object);
                    t_Objects.RefreshTile(pos);
                }

                if (tile.Region != Region.None)
                {
                    var newTile = ScriptableObject.CreateInstance<MapTile>();
                    newTile.Region = tile.Region;
                    t_Regions.SetTile(pos, newTile);
                    newTile.Render(Models.Static.RenderType.Region);
                    t_Regions.RefreshTile(pos);
                }

            }
        }        
    }

    public string SavetoJSON()
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;

        int minY = int.MaxValue;
        int maxY = int.MinValue;

        for(int x = 0; x < _width; x++)//loops thorugh entire map
        {
            for(int y = 0; y < _height; y++)
            {
                var tile = Tiles[x, y];

                if(tile.ObjectType != 0)//if we find a tile that is not empty 
                {
                    if (x > maxX) maxX = x; //we check if our current position is greater then our highest X position and same for Y and min values
                    if (x < minX) minX = x; //this gets us our bounds Rectangle

                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                }

                if (tile.GroundType != 0 && tile.GroundType != 0xff)//if we find a tile that is not empty 
                {
                    if (x > maxX) maxX = x; //we check if our current position is greater then our highest X position and same for Y and min values
                    if (x < minX) minX = x; //this gets us our bounds Rectangle

                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                }

                if (tile.Region != Region.None)//if we find a tile that is not empty 
                {
                    if (x > maxX) maxX = x; //we check if our current position is greater then our highest X position and same for Y and min values
                    if (x < minX) minX = x; //this gets us our bounds Rectangle

                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                }
            }
        }

        //Debug.LogWarning($"Rectangle is Y:{minY}-{maxY} X:{minX}-{maxX}");

        //Don't need this?
        t_Tiles.CompressBounds();
        t_Objects.CompressBounds();
        t_Regions.CompressBounds();


        Bounds bounds = new Bounds();
        bounds.SetMinMax(new Vector3(minX, minY), new Vector3(maxX, maxY));

        _lineRenderer.positionCount = 4;
        _lineRenderer.SetPosition(0, new Vector3(bounds.min.x, bounds.min.y, 0));
        _lineRenderer.SetPosition(1, new Vector3(bounds.max.x, bounds.min.y, 0));
        _lineRenderer.SetPosition(2, new Vector3(bounds.max.x, bounds.max.y, 0));
        _lineRenderer.SetPosition(3, new Vector3(bounds.min.x, bounds.max.y, 0));

        return JSMap.SerializeMap(Tiles, bounds);
    }
    public void SetSize(int height, int width)
    {
        LoadedTiles = new int[width, height];
        
        t_Tiles.ClearAllTiles();
        t_Objects.ClearAllTiles();
        t_Regions.ClearAllTiles();
        
        _height = height;
        _width = width;

        Tiles = new MapTile[width, height];
        _camera.transform.SetPositionAndRotation(new Vector3(_width / 2, _height / 2, -10), Quaternion.identity); //Middle

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Tiles[x, y] = ScriptableObject.CreateInstance<MapTile>();
            }
        }

        _lineRenderer.positionCount = 4;
        _lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        _lineRenderer.SetPosition(1, new Vector3(_width, 0, 0));
        _lineRenderer.SetPosition(2, new Vector3(_width, _height, 0));
        _lineRenderer.SetPosition(3, new Vector3(0, _height, 0));
    }
}

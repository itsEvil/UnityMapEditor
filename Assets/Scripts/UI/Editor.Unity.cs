using Models.Static;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public partial class Editor : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer; //outer bounds
    [SerializeField] private TileDetails _tileDetails;
    [SerializeField] private Sprite s_Highlight;
    [SerializeField] private Tilemap t_Tiles, t_Objects, t_Regions, t_Highlight; //Only to render things on seperate layers
    private Camera _camera;
    private Vector3 _worldMousePos;
    private Vector3Int _mousePosInt;
    private Vector3Int _previousTilePos;
    [SerializeField] private float _speed = 15f;
    private ToolType _toolEquipped = ToolType.Pencil;
    private RenderType _selectedType;
    private int _itemType;
    private void Awake()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        TickMousePosition();
        TickInput();
        TickMouseInput();
        TickCameraSize();
        TickBrushSize();
        TickCameraMovement();
    }
    private bool BoundsCheck(int x, int y)
    {
        if (x < 0 || y < 0 || x>= _width || y >= _height)
            return false;
        return true;
    }
    private bool BoundsCheck(Vector3Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= _width || pos.y >= _height)
            return false;
        return true;
    }
    private void DrawTile(int x, int y)
    {
        Debug.Log($"Drawing to {_selectedType} map with type {_itemType} at x:{x} y:{y}");

        if (_brushSize == 0)
        {
            SetTile(_mousePosInt);
        }
        else
        {
            SetTilesByBrushSize(_mousePosInt);
        }
    }
    public void SetTile(Vector3Int tilePos)
    {
        var previous = ScriptableObject.CreateInstance<MapTile>();

        var tile = Tiles[tilePos.x, tilePos.y];
        
        previous.GroundType = tile.GroundType;
        previous.ObjectType = tile.ObjectType;
        previous.Region = tile.Region;

        Tilemap map = t_Tiles;

        if(_selectedType == RenderType.Tile)
        {
            if (tile.GroundType == _itemType)
                return;

            tile.GroundType = _itemType;
        }
        else if(_selectedType == RenderType.Object)
        {
            if (tile.ObjectType == _itemType)
                return;

            map = t_Objects;

            tile.ObjectType = _itemType;
        }
        else if(_selectedType == RenderType.Region)
        {
            if (tile.Region == (Region)_itemType)
                return;

            map = t_Regions;

            tile.Region = (Region)_itemType;
        }


        tile.Render(_selectedType);

        map.SetTile(tilePos, tile);
        map.RefreshTile(tilePos);

        LoadedTiles[tilePos.x, tilePos.y] = 0;
        Tiles[tilePos.x, tilePos.y] = tile;

        BuildingHistory.Instance.Add(new BuildingHistoryItem(map, Tiles, tilePos, previous, tile, _selectedType));

    }
    public void SetTilesByBrushSize(Vector3Int tilePos)
    {
        SetTiles(tilePos, offsetMaxX: _brushSize, offsetMaxY: _brushSize);
        SetTiles(tilePos, offsetMinX: -_brushSize, offsetMaxY: _brushSize);
        SetTiles(tilePos, offsetMinX: -_brushSize, offsetMinY: -_brushSize);
        SetTiles(tilePos, offsetMaxX: _brushSize, offsetMinY: -_brushSize);
    }
    public void SetTiles(Vector3Int tilePos, int offsetMinX = 0, int offsetMaxX = 0, int offsetMinY = 0, int offsetMaxY = 0)
    {
        if (Helpers.IsOverUi())
            return;

        for (int posX = tilePos.x + offsetMinX; posX <= tilePos.x + offsetMaxX; posX++)
        {
            for (int posY = tilePos.y + offsetMinY; posY <= tilePos.y + offsetMaxY; posY++)
            {
                var pos = new Vector3Int(posX, posY);

                if (!BoundsCheck(pos))
                    return;

                SetTile(pos);
            }
        }
    }
    public void RemoveTiles(Vector3Int tilePos, int offsetMinX = 0, int offsetMaxX = 0, int offsetMinY = 0, int offsetMaxY = 0)
    {
        if (Helpers.IsOverUi())
            return;

        for (int posX = tilePos.x + offsetMinX; posX <= tilePos.x + offsetMaxX; posX++)
        {
            for (int posY = tilePos.y + offsetMinY; posY <= tilePos.y + offsetMaxY; posY++)
            {
                var pos = new Vector3Int(posX, posY);
                
                if (!BoundsCheck(pos))
                    return;

                RemoveTile(pos);
            }
        }
    }
    private void OnEraser()
    {
        Debug.Log($"Trying to erase at {_mousePosInt}");

        RemoveTiles(_mousePosInt, offsetMaxX: _brushSize, offsetMaxY: _brushSize);
        RemoveTiles(_mousePosInt, offsetMinX: -_brushSize, offsetMaxY: _brushSize);
        RemoveTiles(_mousePosInt, offsetMinX: -_brushSize, offsetMinY: -_brushSize);
        RemoveTiles(_mousePosInt, offsetMaxX: _brushSize, offsetMinY: -_brushSize);
    }

    private void RemoveTile(Vector3Int pos)
    {
        var tile = Tiles[pos.x, pos.y];

        var emptyTile = ScriptableObject.CreateInstance<MapTile>();

        switch (_selectedType)
        {
            case RenderType.Tile:
                tile.GroundType = 0xff;
                t_Tiles.SetTile(pos, emptyTile);
                t_Tiles.RefreshTile(_mousePosInt);
                break;
            case RenderType.Object:
                tile.ObjectType = 0;
                t_Objects.SetTile(pos, emptyTile);
                t_Objects.RefreshTile(_mousePosInt);
                break;
            case RenderType.Region:
                tile.Region = Region.None;
                t_Regions.SetTile(pos, emptyTile);
                t_Regions.RefreshTile(_mousePosInt);
                break;
        }

        tile.Render(_selectedType);
        LoadedTiles[pos.x, pos.y] = 0;
    }

    private void LateUpdate()
    {
        var tilePos = t_Highlight.WorldToCell(_mousePosInt);

        if (!Helpers.IsOverUi())
        {
            if (BoundsCheck(tilePos.x, tilePos.y))
            {
                var mapTile = Tiles[tilePos.x, tilePos.y];

                _tileDetails.SetTile(tilePos, mapTile.GroundType, mapTile.ObjectType, mapTile.Region);
            }
            else
            {
                _tileDetails.RemoveTile();
            }
        }

        if (tilePos == _previousTilePos)
            return;

        var tile = ScriptableObject.CreateInstance<HighlightTile>();

        tile.Init(s_Highlight);

        //Center tiles


        DrawHighlight(tilePos, tile);

        _previousTilePos = tilePos;
    }

    private void DrawHighlight(Vector3Int tilePos, HighlightTile tile, int previousSize = -1)
    {
        if (previousSize == -1)
            previousSize = _brushSize;

        if (_brushSize == 0)
        {
            t_Highlight.SetTile(tilePos, tile);
            t_Highlight.SetTile(_previousTilePos, null);
        }
        else
        {
            SetHighlightTiles(null, _previousTilePos, offsetMaxX: previousSize, offsetMaxY: previousSize);
            SetHighlightTiles(null, _previousTilePos, offsetMinX: -previousSize, offsetMaxY: previousSize);
            SetHighlightTiles(null, _previousTilePos, offsetMinX: -previousSize, offsetMinY: -previousSize);
            SetHighlightTiles(null, _previousTilePos, offsetMaxX: previousSize, offsetMinY: -previousSize);

            SetHighlightTiles(tile, tilePos, offsetMaxX: _brushSize, offsetMaxY: _brushSize);
            SetHighlightTiles(tile, tilePos, offsetMinX: -_brushSize, offsetMaxY: _brushSize);
            SetHighlightTiles(tile, tilePos, offsetMinX: -_brushSize, offsetMinY: -_brushSize);
            SetHighlightTiles(tile, tilePos, offsetMaxX: _brushSize, offsetMinY: -_brushSize);
        }
    }

    public void SetHighlightTiles(HighlightTile tile, Vector3Int tilePos, int offsetMinX = 0, int offsetMaxX = 0, int offsetMinY = 0, int offsetMaxY = 0)
    {
        for (int posX = tilePos.x + offsetMinX; posX <= tilePos.x + offsetMaxX; posX++)
        {
            for (int posY = tilePos.y +offsetMinY; posY <= tilePos.y + offsetMaxY; posY++)
            {
                t_Highlight.SetTile(new Vector3Int(posX, posY), tile);
            }
        }
    }
}

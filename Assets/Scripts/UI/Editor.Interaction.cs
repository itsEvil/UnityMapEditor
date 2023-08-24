using Models.Static;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public partial class Editor : MonoBehaviour
{
    private int _brushSize = 3;
    private void TickInput()
    {
        if(Input.GetKey(KeyCode.LeftControl))//Undo
        {
            if(Input.GetKey(KeyCode.Z))
            {
                var pos = BuildingHistory.Instance.GetStepPosition();
                LoadedTiles[pos.x, pos.y] = 0;
                BuildingHistory.Instance.UndoStep();

            }
            if (Input.GetKey(KeyCode.Y))
            {
                var pos = BuildingHistory.Instance.GetStepPosition();
                LoadedTiles[pos.x, pos.y] = 0;
                BuildingHistory.Instance.RedoStep();
            }
        }
    }
    private void TickBrushSize()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        int size = _brushSize;

        if (Input.mouseScrollDelta.y > 0)
        {
            _brushSize++;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            _brushSize--;
        }

        if (_brushSize < 0)
            _brushSize = 0;

        if (_brushSize > 25)
            _brushSize = 25;

        if(size != _brushSize)
        {
            if(_brushSize == 0) t_Highlight.ClearAllTiles();

            var tile = ScriptableObject.CreateInstance<HighlightTile>();

            tile.Init(s_Highlight);

            DrawHighlight(t_Highlight.WorldToCell(_mousePosInt), tile, size > _brushSize ? size : _brushSize);
        }
    }
    private void TickMouseInput()
    {
        //Left click
        if(Input.GetMouseButton(0))
        {
            OnLeftClick();
        }
    }
    public void SetTool(ToolType tool)
    {
        _toolEquipped = tool;
    }    
    private void OnLeftClick()
    {
        switch (_toolEquipped)
        {
            case ToolType.Pencil:
                DrawTile(_mousePosInt.x, _mousePosInt.y);
                break;
            case ToolType.Eraser:
                OnEraser();
                break;
        }
    }
    public void SetItem(RenderType itemPickerType, int type, string name = "")
    {
        _selectedType = itemPickerType;
        _itemType = type;

        Debug.Log($"Setting type to {name}:{_itemType}:{itemPickerType}");
    }
    private void TickCameraMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _camera.transform.position += _speed * Time.deltaTime * Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _camera.transform.position += _speed * Time.deltaTime * Vector3.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _camera.transform.position += _speed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _camera.transform.position += _speed * Time.deltaTime * Vector3.left;
        }

        if (Tiles == null)
            return;

        var sight = SightCircle;

        if (sight == null)
            Debug.LogError("Sight is null!");

        foreach (var p in sight)
        {
            var x = p.X + (int)_camera.transform.position.x;
            var y = p.Y + (int)_camera.transform.position.y;
           
            if (x >= _width || x < 0)
                continue;

            if (y >= _height || y < 0)
                continue;

            if (LoadedTiles[x, y] == 1)
                continue;

            LoadedTiles[x, y] = 1;           

            if (Tiles[x, y] == null)
            {
                Debug.Log($"Skipping tile at {x}:{y} because its null?");
                continue;
            }

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
    private void TickMousePosition()
    {
        _worldMousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        _mousePosInt = new Vector3Int((int)Mathf.Floor(_worldMousePos.x), (int)Mathf.Floor(_worldMousePos.y), 0);
    }

    private void TickCameraSize()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            return;

        float size = _camera.orthographicSize + (Input.mouseScrollDelta.y * 2f);
        if (size < 2)
            size = 2;
        if (size > 30)
            size = 30;

        _camera.orthographicSize = size;
    }
}

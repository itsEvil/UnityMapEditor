using Models.Static;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingHistory : MonoBehaviour
{
    public static BuildingHistory Instance { get; private set; }
    List<BuildingHistoryItem> _items;
    private int _index;
    public void Awake()
    {
        Instance = this;
        _items = new();
        _index = -1;
    }

    public void Add(BuildingHistoryItem item)
    {
        _items.RemoveRange(_index + 1, _items.Count - (_index + 1));
        _items.Add(item);
        _index++;
    }

    public void UndoStep()
    {
        if(_index > -1)
        {
            _items[_index].Undo();
            _index--;
        }
    }
    public Vector3Int GetStepPosition()
    {
        if(_index > -1)
        {
            return _items[_index].GetPos();
        }
        return new Vector3Int();
    }
    public void RedoStep()
    {
        if (_index < _items.Count - 1)
        {
            _index++;
            _items[_index].Redo();
        }
    }
}

public class BuildingHistoryItem
{
    private Tilemap _map;
    private MapTile[,] _array;
    private RenderType _renderType;
    private Vector3Int _position;
    private MapTile _prevTile;
    private MapTile _newTile;

    public BuildingHistoryItem(Tilemap map, MapTile[,] array, Vector3Int position, MapTile prevTile, MapTile newTile, RenderType type)
    {
        _map = map;
        _array = array;
        _position = position;
        _prevTile = prevTile;
        _newTile = newTile;
        _renderType = type;
    }
    public Vector3Int GetPos()
    {
        return _position;
    }
    public void Undo()
    {
        _array[_position.x, _position.y] = _prevTile;
        _map.SetTile(_position, _prevTile);
        _map.RefreshTile(_position);

        _prevTile.Render(_renderType);
    }

    public void Redo() 
    {
        _array[_position.x, _position.y] = _newTile;
        _map.SetTile(_position, _newTile);
        _map.RefreshTile(_position);
        _newTile.Render(_renderType);
    }
}

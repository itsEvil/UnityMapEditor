using Models.Static;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ItemPicker
{
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private Button _searchButton;

    private TileDesc[] _tilesSearched;
    private ObjectDesc[] _objectsSearched;
    private RegionDesc[] _regionsSearched;
    public void OnSearch()
    {
        var text = _input.text;

        Debug.Log($"We tried to search! {text}");

        if(string.IsNullOrWhiteSpace(text))
        {
            ResetItems();
        }
        _currentPage = 0;
        _searchMode = true;
        switch (_itemTypes)
        {
            case RenderType.Tile:
                SearchTiles(text);
                break;
            case RenderType.Object:
                SearchObjects(text);
                break;
            case RenderType.Region:
                SearchRegions(text);
                break;
        }

        UpdatePageText();
    }

    private void SearchRegions(string text)
    {
        _regionsSearched = _regionsDescs
            .AsParallel().WithDegreeOfParallelism(4)
            .Where(x => x.Id.Contains(text, System.StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        _maxPagesRegions = _regionsSearched.Length / ITEMS_PER_PAGE;

        DrawSearchedRegions();
    }

    private void DrawSearchedRegions()
    {
        var max = _currentPage * ITEMS_PER_PAGE;
        for (int i = 0; i < ITEMS_PER_PAGE; i++)
        {
            if (i + max >= _regionsSearched.Length)
            {
                _items[i].gameObject.SetActive(false);
                continue;
            }

            var regionDesc = _regionsSearched[i + max];

            if (!_items[i].gameObject.activeSelf)
                _items[i].gameObject.SetActive(true);

            _items[i].Init(regionDesc.Type, regionDesc.Id, _itemTypes);
            _items[i].SetColor(Color.white);
            _items[i].SetSprite(Cache.Instance.GetRegionSprite());
        }
    }

    private void SearchObjects(string text)
    {
        _objectsSearched = _objectDescs
            .AsParallel().WithDegreeOfParallelism(4)
            .Where(x => x.Id.Contains(text, System.StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        _maxPagesObjects = _objectsSearched.Length / ITEMS_PER_PAGE;
        DrawSearchedObjects();
    }

    private void DrawSearchedObjects()
    {
        var max = _currentPage * ITEMS_PER_PAGE;
        for (int i = 0; i < ITEMS_PER_PAGE; i++)
        {
            if (i + max >= _objectsSearched.Length)
            {
                _items[i].gameObject.SetActive(false);
                continue;
            }

            var objDesc = _objectsSearched[i + max];

            if (!_items[i].gameObject.activeSelf)
                _items[i].gameObject.SetActive(true);

            _items[i].Init(objDesc.Type, objDesc.Id, _itemTypes);
            _items[i].SetColor(Color.white);

            if (objDesc.TextureData.Invisible)
            {
                _items[i].SetSprite(Cache.Instance.GetNoSprite());
            }
            else
                _items[i].SetSprite(objDesc.TextureData.Texture);

            _items[i].SetSprite(objDesc.TextureData.Texture);
        }
    }

    private void SearchTiles(string text)
    {
        _tilesSearched = _tilesDescs
            .AsParallel().WithDegreeOfParallelism(4)
            .Where(x => x.Id.Contains(text, System.StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        _maxPagesTiles = _tilesSearched.Length / ITEMS_PER_PAGE;

        DrawSearchedTiles();
    }

    private void DrawSearchedTiles()
    {
        var max = _currentPage * ITEMS_PER_PAGE;
        for (int i = 0; i < ITEMS_PER_PAGE; i++)
        {
            if (i + max >= _tilesSearched.Length)
            {
                _items[i].gameObject.SetActive(false);
                continue;
            }

            if (!_items[i].gameObject.activeSelf)
                _items[i].gameObject.SetActive(true);

            var tileDesc = _tilesSearched[i + max];

            _items[i].Init(tileDesc.Type, tileDesc.Id, _itemTypes);
            _items[i].SetColor(Color.white);
            _items[i].SetSprite(tileDesc.TextureData.Texture);
        }
    }

    private void ResetItems()
    {
        LoadItems();
    }
}

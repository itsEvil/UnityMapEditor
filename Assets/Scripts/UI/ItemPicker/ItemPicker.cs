using Models.Static;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ItemPicker : MonoBehaviour
{
    public static ItemPicker Instance { get; private set; }
    private const int ITEMS_PER_PAGE = 60;
    [SerializeField] private Button _tilesButton, _objectsButton, _regionsButton, _closeButton, _previousPageButton, _nextPageButton;
    [SerializeField] private TMP_Text _titleText, _pageText;
    [SerializeField] private Transform _container, _startPosition;
    [SerializeField] private Item _itemPrefab;
    private Item[] _items;
    private int _currentPage = 0;
    private RenderType _itemTypes = RenderType.Tile;

    private int _maxPagesTiles;
    private int _maxPagesObjects;
    private int _maxPagesRegions;


    private TileDesc[] _tilesDescs;
    private ObjectDesc[] _objectDescs;
    private RegionDesc[] _regionsDescs;
    private Editor _editor;

    private bool _searchMode = false;

    public void Init(Editor editor)
    {
        _editor = editor;
        SetItem(RenderType.Tile, 0xff); //Empty
    }
    private void Awake()
    {
        Instance = this;

        _items = new Item[ITEMS_PER_PAGE];
        int x_Counter = 0;
        int y_Counter = 0;

        for(int i = 0; i < ITEMS_PER_PAGE; i++)
        {
            if(x_Counter == 5)
            {
                x_Counter = 0;
                y_Counter++;
            }
            var obj = Instantiate(_itemPrefab, _container);

            obj.transform.SetPositionAndRotation(
                    new Vector3(_startPosition.position.x + (x_Counter * 48) + 24,
                                _startPosition.position.y - (y_Counter * 48) - 24,
                                0),
                                Quaternion.identity);

            x_Counter++;

            _items[i] = obj;
        }
    }
    private void Start()
    {
        //should prob just do this inside of AssetLibrary

        _objectsSearched = new ObjectDesc[0];
        _tilesSearched = new TileDesc[0];
        _regionsSearched = new RegionDesc[0];

        _tilesDescs = AssetLibrary.Type2TileDesc.Values
            .ToArray();

        _objectDescs = AssetLibrary.Type2ObjectDesc.Values
            .ToArray();

        _regionsDescs = AssetLibrary.Type2RegionDesc.Values
            .ToArray();

        _pageText.text = $"Page {_currentPage}/{_maxPagesTiles}";

        Debug.Log($"Tiles:{_tilesDescs.Length} Objects:{_objectDescs.Length} Regions: {_regionsDescs.Length}");

        LoadItems();
    }
    private void LoadItems()
    {
        _searchMode = false;

        _maxPagesTiles = _tilesDescs.Length / ITEMS_PER_PAGE;
        _maxPagesObjects = _objectDescs.Length / ITEMS_PER_PAGE;
        _maxPagesRegions = _regionsDescs.Length / ITEMS_PER_PAGE;

        int length = _tilesDescs.Length;
        int type = 0xff;
        string name = "";
        Sprite sprite = AssetLibrary.Id2TileDesc["Empty"].TextureData.Texture;

        Color color = Color.white;
        var max = _currentPage * ITEMS_PER_PAGE;

        if(_itemTypes == RenderType.Object)
        {
            length = _objectDescs.Length;
        }
        else if(_itemTypes == RenderType.Region)
        {
            length = _regionsDescs.Length;
        }


        for (int i = 0; i < ITEMS_PER_PAGE; i++)
        {
            if (i + max >= length)
            {
                _items[i].gameObject.SetActive(false);
                continue;
            }

            if(_itemTypes == RenderType.Tile)
            {
                var tileDesc = _tilesDescs[i + max];
                type = tileDesc.Type;
                sprite = tileDesc.TextureData.Texture;
                name = tileDesc.Id;
                color = Color.white;
            }
            else if(_itemTypes == RenderType.Object)
            {
                var objDesc = _objectDescs[i + max];
                
                if(objDesc == null)
                {
                    Debug.LogWarning($"{i + max} objDesc was null!");
                    continue;
                }
                type = objDesc.Type;
                name = objDesc.Id;
                
                color = Color.white;

                if (objDesc.TextureData.Invisible)
                {
                    sprite = Cache.Instance.GetNoSprite();
                }
                else
                    sprite = objDesc.TextureData.Texture;
            }
            else if(_itemTypes == RenderType.Region)
            {
                var regDesc = _regionsDescs[i + max];
                type = regDesc.Type;
                name = regDesc.Id;
                sprite = Cache.Instance.GetRegionSprite();
                color = regDesc.Color;
            }

            if (!_items[i].gameObject.activeSelf)
                _items[i].gameObject.SetActive(true);

            _items[i].Init(type, name, _itemTypes);
            _items[i].SetColor(color);
            _items[i].SetSprite(sprite);
        }
    }
    private void OnClose()
    {
        transform.localScale = Vector3.zero;
    }
    private void UpdatePageText()
    {
        switch (_itemTypes)
        {
            case RenderType.Tile:
                if (_currentPage > _maxPagesTiles)
                    _currentPage = _maxPagesTiles;

                _pageText.text = $"Page {_currentPage}/{_maxPagesTiles}";
                break;
            case RenderType.Object:
                if (_currentPage > _maxPagesObjects)
                    _currentPage = _maxPagesObjects;

                _pageText.text = $"Page {_currentPage}/{_maxPagesObjects}";
                break;
            case RenderType.Region:
                if (_currentPage > _maxPagesRegions)
                    _currentPage = _maxPagesRegions;

                _pageText.text = $"Page {_currentPage}/{_maxPagesRegions}";
                break;
        }
    }
    private void OnNextPage()
    {
        _currentPage++;
        switch (_itemTypes)
        {
            case RenderType.Tile:
                if (_currentPage > _maxPagesTiles)
                    _currentPage = _maxPagesTiles;

                _pageText.text = $"Page {_currentPage}/{_maxPagesTiles}";

                if (_searchMode)
                    DrawSearchedTiles();

                break;
            case RenderType.Object:
                if (_currentPage > _maxPagesObjects)
                    _currentPage = _maxPagesObjects;

                _pageText.text = $"Page {_currentPage}/{_maxPagesObjects}";

                if (_searchMode)
                    DrawSearchedObjects();

                break;
            case RenderType.Region:
                if (_currentPage > _maxPagesRegions)
                    _currentPage = _maxPagesRegions;

                _pageText.text = $"Page {_currentPage}/{_maxPagesRegions}";

                if (_searchMode)
                    DrawSearchedRegions();

                break;
        }
        
        if(!_searchMode)
            LoadItems();
    }

    private void OnPreviousPage()
    {
        _currentPage--;
        if (_currentPage < 0)
            _currentPage = 0;

        switch (_itemTypes)
        {
            case RenderType.Tile:
                _pageText.text = $"Page {_currentPage}/{_maxPagesTiles}";
                if (_searchMode)
                    DrawSearchedTiles();
                break;
            case RenderType.Object:
                _pageText.text = $"Page {_currentPage}/{_maxPagesObjects}";
                if (_searchMode)
                    DrawSearchedObjects();
                break;
            case RenderType.Region:
                _pageText.text = $"Page {_currentPage}/{_maxPagesRegions}";
                if (_searchMode)
                    DrawSearchedRegions();
                break;
        }

        if(!_searchMode)
            LoadItems();
    }

    private void OnRegions()
    {
        _currentPage = 0;
        _titleText.text = "Regions";
        _pageText.text = $"Page {_currentPage}/{_maxPagesRegions}";
        _itemTypes = RenderType.Region;
        LoadItems();

        SetItem(RenderType.Region, _regionsDescs[0].Type, _regionsDescs[0].Id);
    }

    private void OnObjects()
    {
        _currentPage = 0;
        _titleText.text = "Objects";
        _pageText.text = $"Page {_currentPage}/{_maxPagesObjects}";
        _itemTypes = RenderType.Object;
        LoadItems();

        SetItem(RenderType.Object, _objectDescs[0].Type, _objectDescs[0].Id);
    }

    private void OnTiles()
    {
        _currentPage = 0;
        _titleText.text = "Tiles";
        _pageText.text = $"Page {_currentPage}/{_maxPagesTiles}";
        _itemTypes = RenderType.Tile;
        LoadItems();
        SetItem(RenderType.Tile, _tilesDescs[0].Type, _tilesDescs[0].Id);
    }
    private void OnEnable()
    {
        _tilesButton.onClick.AddListener(OnTiles);
        _objectsButton.onClick.AddListener(OnObjects);
        _regionsButton.onClick.AddListener(OnRegions);
        _previousPageButton.onClick.AddListener(OnPreviousPage);
        _nextPageButton.onClick.AddListener(OnNextPage);
        _closeButton.onClick.AddListener(OnClose);

        _searchButton.onClick.AddListener(OnSearch);
    }
    private void OnDisable()
    {
        _tilesButton.onClick.RemoveAllListeners();
        _objectsButton.onClick.RemoveAllListeners();
        _regionsButton.onClick.RemoveAllListeners();
        _previousPageButton.onClick.RemoveAllListeners();
        _nextPageButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();

        _searchButton.onClick.RemoveAllListeners();
    }
    public void SetItem(RenderType pickerType, int type, string name = "")
    {
        _editor.SetItem(pickerType, type, name);
    }
}

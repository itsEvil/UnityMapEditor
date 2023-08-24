using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EditorScreenController : UIScene
{
    [SerializeField] private Button _close, _save, _itemPickerButton, _toolsButton;
    [SerializeField] private Editor _editor;
    [SerializeField] private ItemPicker _itemPicker;
    [SerializeField] private ToolPicker _toolPicker;
    private string _completeMapData = "This is the map data";
    private void Start()
    {
        _itemPicker.Init(_editor);
    }
    private void OnTools()
    {
        _toolPicker.gameObject.SetActive(!_toolPicker.gameObject.activeSelf);
    }
    public override void Load(object data = null)
    {
        MapInfo mapInfo = (MapInfo)data;

        if (mapInfo == null) throw new Exception("MapInfo is null!"); //why is it null?

        if(!mapInfo.IsNewMap)
        {
            //load map from json
            _editor.LoadMap(Map.LoadedMap);
        }
        else
        {
            //create blank map with set size
            _editor.SetSize(mapInfo.Height, mapInfo.Width);
        }

        OnItemPicker();

        base.Load(data);
    }
    private void OnItemPicker()
    {
        if (_itemPicker.transform.localScale == Vector3.zero)
            _itemPicker.transform.localScale = Vector3.one;
        else _itemPicker.transform.localScale = Vector3.zero;
    }
    private void OnSave()
    {
        _completeMapData = _editor.SavetoJSON();

        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "map", "map");
        if(!string.IsNullOrEmpty(path)) 
        {
            File.WriteAllText(path, _completeMapData);
        }
            
    }
    private void OnClose()
    {
        Debug.Log("Close pressed!");

        UIController.Instance.ChangeView(UIController.View.Menu);
    }
    private void OnEnable()
    {
        _close.onClick.AddListener(OnClose);
        _save.onClick.AddListener(OnSave);
        _itemPickerButton.onClick.AddListener(OnItemPicker);
        _toolsButton.onClick.AddListener(OnTools);
    }
    private void OnDisable()
    {
        _close.onClick.RemoveAllListeners();
        _save.onClick.RemoveAllListeners();
        _itemPickerButton.onClick.RemoveAllListeners();
        _toolsButton.onClick.RemoveAllListeners();
    }
}

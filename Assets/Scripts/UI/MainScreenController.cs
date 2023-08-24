using SFB;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainScreenController : UIScene
{
    [SerializeField] private Button b_Create, b_Load, b_Close;
    [SerializeField] private TMP_InputField if_Width, if_Height;
    private int _width;
    private int _height;

    private void Awake()
    {
        b_Create.onClick.AddListener(OnCreate);   
        b_Load.onClick.AddListener(OnLoad);
        b_Close.onClick.AddListener(OnClose);
    }

    private void OnClose()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnLoad()
    {
        var extensions = new[] {
            new ExtensionFilter("Map files", "map", "jm", "wmap"),
            new ExtensionFilter("All Files", "*" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

        if(paths.Length > 0)
        {
            Uri uri = new Uri(paths[0]);
            StartCoroutine(OutputRoutineOpen(uri.AbsoluteUri));
        }
    }
    
    private IEnumerator OutputRoutineOpen(string absoluteUri)
    {
        UnityWebRequest www = UnityWebRequest.Get(absoluteUri);
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW Error" + www.error);
        }
        else
        {
            //Parse loaded info 

            Debug.Log(www.downloadHandler.text);

            if(absoluteUri.EndsWith("jm"))
            {
                Map.LoadedMap = JSMap.LoadOldJson(www.downloadHandler.text);
            }
            else if(absoluteUri.EndsWith("wmap"))
            {
                Map.LoadedMap = new WMap(www.downloadHandler.data);
            }
            else
            {
                Map.LoadedMap = new JSMap(www.downloadHandler.text);
            }

            if (Map.LoadedMap == null)
                Debug.LogError("Failed to load map");

            MapInfo info = new()
            {
                IsNewMap = false,
                Name = absoluteUri,
            };

            UIController.Instance.ChangeView(UIController.View.Editor, info);
        }
    }

    private void OnCreate()
    {
        if (int.TryParse(if_Height.text, out int height))
        {
            _height = height;
        }
        else _height = 128;

        if (int.TryParse(if_Width.text, out int width))
        {
            _width = width;
        }
        else _width = 128;

        var mapInfo = new MapInfo()
        {
            IsNewMap = true,
            Name = "map",
            Width = _width,
            Height = _height,
        };

        UIController.Instance.ChangeView(UIController.View.Editor, mapInfo);
    }
}

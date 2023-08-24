using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private MainScreenController uis_MainView;
    [SerializeField] private EditorScreenController uis_EditorView;

    private UIScene activeView;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        ChangeView(View.Menu);
    }
    public void ChangeView(View view, object data = null)
    {
        UIScene newView;
        switch (view)
        {
            case View.Menu:
                newView = uis_MainView;
                break;
            case View.Editor:
                newView = uis_EditorView;
                break;
            default:
                throw new Exception($"{view} not yet implemented");
        }

        activeView?.gameObject.SetActive(false);
        activeView = newView;
        activeView?.gameObject.SetActive(true);
        activeView.Load(data);
    }
    public enum View
    {
        Menu,
        Editor,
    }
}

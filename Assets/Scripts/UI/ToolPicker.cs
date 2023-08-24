using Models.Static;
using UnityEngine;
using UnityEngine.UI;

public class ToolPicker : MonoBehaviour
{
    [SerializeField] private Button _pencilB, _eraserB;
    [SerializeField] private Editor _editor;
    public void OnEnable()
    {
        _pencilB.onClick.AddListener(OnPencil);
        _eraserB.onClick.AddListener(OnEraser);
    }
    public void OnDisable()
    {
        _pencilB.onClick.RemoveAllListeners();
        _eraserB.onClick.RemoveAllListeners();
    }
    private void OnEraser()
    { 
        _editor.SetTool(ToolType.Eraser);
    }
    private void OnPencil()
    {
        _editor.SetTool(ToolType.Pencil);
    }
}

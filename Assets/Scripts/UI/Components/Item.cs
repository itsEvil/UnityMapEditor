using Models.Static;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image i_Icon;
    [SerializeField] private Button b_Click;
    private ItemPicker _parent => ItemPicker.Instance;
    private int _type;
    private string _name;
    private RenderType _pickerType;
    public void Init(int type, string name, RenderType pickerType)
    {
        _name = name;
        _type = type;
        _pickerType = pickerType;
    }
    public void SetSprite(Sprite sprite)
    {
        i_Icon.sprite = sprite;
    }
    public void SetColor(Color color) 
    {
        i_Icon.color = color;
    }
    private void OnEnable()
    {
        b_Click.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        b_Click.onClick.RemoveAllListeners();
    }
    private void OnClick()
    {
        TileDetails.Instance.SetText($"ItemPicker: {_name}");
        _parent.SetItem(_pickerType, _type, _name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TileDetails.Instance.SetText($"ItemPicker: {_name}");
    }
}

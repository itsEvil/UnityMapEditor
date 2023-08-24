using Models.Static;
using TMPro;
using UnityEngine;

public class TileDetails : MonoBehaviour
{
    public static TileDetails Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private TMP_Text t_Text;
    public void RemoveTile()
    {
        transform.localScale = Vector3.zero;
    }
    public void SetText(string text)
    {
        t_Text.SetText(text);
        transform.localScale = Vector3.one;
    }
    public void SetTile(Vector3Int pos, int tile, int obj, Region region = Region.None)
    {
        if (Helpers.IsOverUi())
            return;

        string tileName = "";
        string objectName = "";
        string regionName = $"{region} ({(int)region})";
        if(tile != 0)
        {
            TileDesc tDesc = AssetLibrary.GetTileDesc(tile);
            tileName = tDesc.Id;
        }

        if(obj != 0)
        {
            ObjectDesc oDesc = AssetLibrary.Type2ObjectDesc[obj];
            objectName = oDesc.DisplayId;
        }

        t_Text.SetText($"Tile: {tileName}\nObject: {objectName}\nRegion: {regionName}\nX:{pos.x} Y:{pos.y}");

        transform.localScale = Vector3.one;
    }
}

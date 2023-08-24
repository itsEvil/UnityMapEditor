using System;
using System.Xml.Linq;
using UnityEngine;
//

public sealed class AssetLoader : MonoBehaviour
{
    private void Start()
    {
        LoadSpriteSheets();
        LoadXmls();
        Destroy(gameObject);
    }

    private void LoadSpriteSheets()
    {
        var spritesXml = XElement.Parse(Resources.Load<TextAsset>("Sprite Sheets/SpriteSheets").text);

        foreach (var sheetXml in spritesXml.Elements("Sheet"))
        {
            var sheetData = new SpriteSheetData(sheetXml);
            var texture = Resources.Load<Texture2D>($"Sprite Sheets/{sheetData.SheetName}");
            try
            {
                if (sheetData.IsAnimation())
                {
                    AssetLibrary.AddAnimations(texture, sheetData);
                }
                else
                {
                    AssetLibrary.AddImages(texture, sheetData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to add {sheetData.Id}\n{e}\n{e.StackTrace}");
            }
        }
    }
    private void LoadXmls()
    {
        var xmlAssets = Resources.LoadAll<TextAsset>("Xmls");

        foreach (var xmlAsset in xmlAssets)
        {
            var xml = XElement.Parse(xmlAsset.text);
            AssetLibrary.ParseXml(xml);
        }

        Debug.Log($"Loaded {AssetLibrary.Type2ObjectDesc.Count} objects");
        Debug.Log($"Loaded {AssetLibrary.Type2TileDesc.Count} tiles");
        Debug.Log($"Loaded {AssetLibrary.Type2RegionDesc.Count} regions");
    }
}
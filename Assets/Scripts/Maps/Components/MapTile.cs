using Models.Static;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class MapTile : Tile
{
    public int GroundType;
    public int ObjectType;
    public Region Region;
    public string Key;
    public MapTile()
    {
        GroundType = 255;
    }
    public void Render(RenderType renderType)
    {
        switch (renderType)
        {
            case RenderType.None:
                sprite = null;
                color = Color.white;
                break;
            case RenderType.Tile:
                sprite = AssetLibrary.Type2TileDesc[GroundType].TextureData.Texture;
                if (Region == Region.None) color = Color.white;
                break;
            case RenderType.Object:
                
                if(AssetLibrary.Type2ObjectDesc.TryGetValue(ObjectType, out var desc))
                {
                    sprite = desc.TextureData.Texture;
                }
                //sprite = AssetLibrary.Type2ObjectDesc[ObjectType].TextureData.Texture;
                if (Region == Region.None) color = Color.white;
                break;
            case RenderType.Region:
                sprite = Cache.Instance.GetRegionSprite();
                color = AssetLibrary.Type2RegionDesc[MiscUtils.GetRegionType(Region)].Color;
                break;
        }
    }
    public bool IsEmpty()
    {
        if (GroundType != 0xff || ObjectType != 0 || Region != Region.None)
            return false;

        return true;
    }
}

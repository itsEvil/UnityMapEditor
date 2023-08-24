using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Models.Static;
using UnityEngine;
using Utils;

public static class AssetLibrary
{
    private static readonly Dictionary<string, List<CharacterAnimation>> Animations =
        new Dictionary<string, List<CharacterAnimation>>();
    
    private static readonly Dictionary<string, List<Sprite>> Images = new Dictionary<string, List<Sprite>>();

    private static Dictionary<int, ObjectDesc> _type2ObjectDesc = new Dictionary<int, ObjectDesc>();
    private static Dictionary<string, ObjectDesc> _id2ObjectDesc = new Dictionary<string, ObjectDesc>();

    public static ReadOnlyDictionary<string, ObjectDesc> Id2ObjectDesc;
    public static ReadOnlyDictionary<int, ObjectDesc> Type2ObjectDesc;

    public static readonly Dictionary<int, TileDesc> Type2TileDesc                  =   new Dictionary<int, TileDesc>();
    public static readonly Dictionary<string, TileDesc> Id2TileDesc                  =   new Dictionary<string, TileDesc>();
    public static readonly Dictionary<int, RegionDesc> Type2RegionDesc              =   new Dictionary<int, RegionDesc>();

    public static void AddAnimations(Texture2D texture, SpriteSheetData data)
    {
        if (!Animations.ContainsKey(data.Id))
            Animations[data.Id] = new List<CharacterAnimation>();

        //Debug.Log($"Animation :{data.SheetName}:{data.Id}");

        for (var y = texture.height; y >= 0; y -= data.AnimationHeight)
        {
            for (var x = 0; x < data.AnimationWidth; x += data.AnimationWidth)
            {
                var rect = new Rect(x, y, data.AnimationWidth, data.AnimationHeight);
                var frames = SpriteUtils.CreateSprites(texture, rect, data.ImageWidth, data.ImageHeight);
                var animation = new CharacterAnimation(frames, data.StartFacing);

                Animations[data.Id].Add(animation);
            }
        }
        
    }
    public static void AddImages(Texture2D texture, SpriteSheetData data)
    {
        if (!Images.ContainsKey(data.Id))
            Images[data.Id] = new List<Sprite>();

        var rect = new Rect(0, texture.height, texture.width, texture.height);
        Images[data.Id] = SpriteUtils.CreateSprites(texture, rect, data.ImageWidth, data.ImageHeight);
    }
    public static void ParseXml(XElement xml)
    {
        foreach (var objectXml in xml.Elements("Object"))
        {
            var classType = objectXml.ParseEnum("Class", ObjectType.None);

            if (classType == ObjectType.Equipment ||
                classType == ObjectType.Projectile ||
                classType == ObjectType.Skin ||
                classType == ObjectType.Dye ||
                classType == ObjectType.Player ||
                classType == ObjectType.None)
                continue;

            var id = objectXml.ParseString("@id");
            var type = objectXml.ParseUshort("@type");
            
            try
            {
                var objectType = objectXml.ParseEnum("Class", ObjectType.None);
        
                if (objectType == ObjectType.None)
                    continue;
        
                var desc = _id2ObjectDesc[id] = _type2ObjectDesc[type] = new ObjectDesc(objectXml, id, type);
            }
            catch (Exception e)
            {
                
                Debug.LogWarning($"Unable to add item {id}");
                Debug.LogError($"item {id} error: {e} ");
            }
        
        }

        Id2ObjectDesc = new ReadOnlyDictionary<string, ObjectDesc>(_id2ObjectDesc);
        Type2ObjectDesc = new ReadOnlyDictionary<int, ObjectDesc>(_type2ObjectDesc);
        
        foreach (var groundXml in xml.Elements("Ground"))
        {
            var id = groundXml.ParseString("@id");
            var type = groundXml.ParseUshort("@type");

            Id2TileDesc[id] = Type2TileDesc[type] = new TileDesc(groundXml, id, type);
        }

        foreach(var regionXml in xml.Elements("Region"))
        {
            var id = regionXml.ParseString("@id");
            var type = regionXml.ParseUshort("@type");
            Type2RegionDesc[type] = new RegionDesc(regionXml, id , type);
        } 
    }
    public static Sprite GetTileImage(int type)
    {
        return Type2TileDesc[type].TextureData.GetTexture();
    }

    public static List<Sprite> GetImageSet(string sheetName)
    {
        return Images[sheetName];
    }

    public static Sprite GetImage(string sheetName, int index)
    {
        if (sheetName.Contains("invisible"))
            return Cache.Instance.NoSprite;

#if UNITY_EDITOR
        try
        {
            return Images[sheetName][index];
        }
        catch(Exception e)
        {
            Debug.LogError($"Sheet name {sheetName} index {index} Error {e}");
        }
#endif
        return Images[sheetName][index];
    }

    public static CharacterAnimation GetAnimation(string sheetName, int index)
    {
        return Animations[sheetName][index];
    }
    public static TileDesc GetTileDesc(int type)
    {
        if(Type2TileDesc.TryGetValue(type, out TileDesc val))
        {
            return val;
        }
        Debug.LogError($"Tile {type} not found in the gameData! Using grass tile!");
        return Type2TileDesc[0x46]; //Light grass
    }
}

public readonly struct SpriteSheetData
{
    public readonly string Id;
    public readonly string SheetName;
    public readonly int AnimationWidth;
    public readonly int AnimationHeight;
    public readonly Facing StartFacing;
    public readonly int ImageWidth;
    public readonly int ImageHeight;

    public SpriteSheetData(XElement xml)
    {
        Id = xml.ParseString("@id");
        SheetName = xml.ParseString("@sheetName", Id);

        var animationSize = xml.ParseIntArray("AnimationSize", "x", new [] {0, 0});
        AnimationWidth = animationSize[0];
        AnimationHeight = animationSize[1];
        StartFacing = xml.ParseEnum("StartDirection", Facing.Right);

        var imageSize = xml.ParseIntArray("ImageSize", "x", new [] {0, 0});
        ImageWidth = imageSize[0];
        ImageHeight = imageSize[1];
    }

    public bool IsAnimation()
    {
        return AnimationWidth != 0 && AnimationHeight != 0;
    }
}

public enum Facing
{
    Up,
    Down,
    Left,
    Right
}
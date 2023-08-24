using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Models.Static
{
    public enum RenderType
    {
        None,
        Tile,
        Object,
        Region
    }
    public enum ToolType
    {
        Pencil,
        Eraser, 
    }
    public enum ObjectType
    {
        GameObject,
        Equipment,
        Skin,
        Dye,
        BountyBoard,
        Campaign,
        Character,
        CharacterChanger,
        ClosedGiftChest,
        ConnectedWall,
        Container,
        Forge,
        GameLore,
        GuildChronicle,
        GuildHallPortal,
        GuildRegister,
        GuildMerchant,
        GuildBoard,
        GuildStore,
        GuildList,
        MarketObject,
        Merchant,
        NameChanger,
        NexusShrine,
        OneWayContainer,
        Player,
        Portal,
        PotionStorage,
        Projectile,
        ReskinVendor,
        RuneStation,
        Stalagmite,
        VaultChest,
        WikiObject,
        SandboxProjectile,
        ClosedVaultChest,
        PetUpgrader,
        CaveWall,
        Sign,

        None = 255,
    }
    public class ObjectDesc
    {
        public readonly string Id;
        public readonly int Type;

        public readonly string DisplayId;

        public readonly bool Static;
        public readonly ObjectType Class;

        public readonly TextureData TextureData;
        public readonly TextureData TopTextureData;
        public readonly TextureData Portrait;
        public readonly AnimationsData AnimationsData;
        //public readonly MeshData MeshData;
        public ObjectDesc(XElement xml, string id, int type)
        {
            Id = id;
            Type = type;

            TextureData = new TextureData(xml);
            DisplayId = xml.ParseString("DisplayId", Id);

            Static = xml.ParseBool("Static");
            Class = xml.ParseEnum("Class", ObjectType.None);
        }
    }
    public class BasicDesc
    {
        public readonly string Name;
        public readonly int Type;
        public readonly string Description;
        public BasicDesc(XElement e, string id, ushort type)
        {
            Name = id;
            Type = type;
            Description = e.ParseString("Description", "Empty");
        }
    }
    public sealed class TileDesc
    {
        public readonly XElement Xml;

        public readonly string Id;
        public readonly ushort Type;
        public readonly TextureData TextureData;
        public readonly bool NoWalk;
        public readonly int Damage;
        public readonly float Speed;
        public readonly bool Sinking;
        public readonly bool Sink;
        public readonly bool Push;
        public readonly float DX;
        public readonly float DY;

        public readonly int BlendPriority;
        public readonly int CompositePriority;
        public readonly bool HasEdge;
        public readonly TextureData EdgeTextureData;
        public readonly TextureData CornerTextureData;
        public readonly TextureData InnerCornerTextureData;
        public readonly bool SameTypeEdgeMode;

        private Sprite[] _edges;
        private Sprite[] _innerCorners;

        public TileDesc(XElement e, string id, ushort type)
        {
            Xml = e;

            Id = id;
            Type = type;
            TextureData = new TextureData(e);
            NoWalk = e.ParseBool("NoWalk");
            Damage = e.ParseInt("Damage");
            Speed = e.ParseFloat("Speed", 1.0f);
            Sinking = e.ParseBool("Sinking");
            Sink = e.ParseBool("Sink");
            if (Push = e.ParseBool("Push"))
            {
                DX = e.Element("Animate").ParseFloat("@dx") / 1000f;
                DY = e.Element("Animate").ParseFloat("@dy") / 1000f;
            }

            BlendPriority = e.ParseInt("BlendPriority", -1);
            CompositePriority = e.ParseInt("CompositePriority");
            if (HasEdge = e.Element("Edge") != null)
            {
                EdgeTextureData = new TextureData(e.Element("Edge"));
                if (e.Element("Corner") != null)
                {
                    CornerTextureData = new TextureData(e.Element("Corner"));
                }
                if (e.Element("InnerCorner") != null)
                {
                    InnerCornerTextureData = new TextureData(e.Element("InnerCorner"));
                }
            }

            SameTypeEdgeMode = e.ParseBool("SameTypeEdgeMode");
        }

        public Sprite[] GetEdges()
        {
            if (!HasEdge || _edges != null)
                return _edges;

            _edges = new Sprite[9];
            _edges[3] = EdgeTextureData.GetTexture();
            _edges[1] = SpriteUtils.Rotate(_edges[3], 3);
            _edges[5] = SpriteUtils.Rotate(_edges[3], 2);
            _edges[7] = SpriteUtils.Rotate(_edges[3], 1);
            if (CornerTextureData != null)
            {
                _edges[0] = SpriteUtils.CreateSingleTextureSprite(CornerTextureData.GetTexture());
                _edges[2] = SpriteUtils.Rotate(_edges[0], 1);
                _edges[8] = SpriteUtils.Rotate(_edges[0], 2);
                _edges[6] = SpriteUtils.Rotate(_edges[0], 3);
            }

            return _edges;
        }

        public Sprite[] GetInnerCorners()
        {
            if (InnerCornerTextureData == null || _innerCorners != null)
                return _innerCorners;

            _innerCorners = _edges.ToArray();
            _innerCorners[0] = SpriteUtils.CreateSingleTextureSprite(InnerCornerTextureData.GetTexture());
            _innerCorners[2] = SpriteUtils.Rotate(_innerCorners[0], 1);
            _innerCorners[8] = SpriteUtils.Rotate(_innerCorners[0], 2);
            _innerCorners[6] = SpriteUtils.Rotate(_innerCorners[0], 3);

            return _innerCorners;
        }
    }
    public enum Region
    {
        None,
        Spawn,
        Regen,
        BlocksSight,
        Note,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4,
        Enemy5,
        Enemy6,
        Decoration1,
        Decoration2,
        Decoration3,
        Decoration4,
        Decoration5,
        Decoration6,
        Trigger1,
        Callback1,
        Trigger2,
        Callback2,
        Trigger3,
        Callback3,
        VaultChest,
        GiftChest,
        VaultPortal,
        RealmPortal,
        GuildPortal,
        Store1,
        Store2,
        Store3,
        Store4,
        Store5,
        Store6,
        Vault,
        Loot,
        Defender,
        Defender1,
        Defender2,
        Defender3,
        Hallway,
        Enemy,
        Hallway1,
        Hallway2,
        Hallway3,
        Store7,
        Store8,
        Store9,
        Store10,
        Store11,
        Store12,
        Store13,
        Store14,
        Store15,
        Store16,
        Store17,
        Store18,
        Store19,
        Store20,
        Store21,
        Store22,
        Store23,
        Store24,
        PetRegion,
        OutsideArena,
        ItemSpawnPoint,
        ArenaCentralSpawn,
        ArenaEdgeSpawn,
        Store25,
        Store26,
        Store27,
        Store28,
        Store29,
        Store30,
        Store31,
        Store32,
        Store33,
        Store34,
        Store35,
        Store36,
        Store37,
        Store38,
        Store39,
        Store40,
        MarketPortal,
        BazaarPortal,
        QuestMonsterRegion
    }
    public sealed class RegionDesc
    {
        public readonly string Id;
        public readonly ushort Type;
        public readonly Color32 Color;
        public readonly Region RegionValue;
        public RegionDesc(XElement e, string id, ushort type)
        {
            Id = id;
            Type = type;
            Color = MiscUtils.ToColor((int)e.ParseUInt("Color"));

            RegionValue = (Region)Enum.Parse(typeof(Region), Id);
        }
    }
}
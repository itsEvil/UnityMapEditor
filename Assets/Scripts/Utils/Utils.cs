using Models.Static;
using UnityEngine;

namespace Utils
{
    public static class MiscUtils
    {
        public static Color32 ToColor(int HexVal, byte alpha = 120)
        {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, alpha);
        }

        public static ushort GetRegionType(Region region) // too lazy atm
        {
            switch (region)
            {
                case Region.None:
                    break;
                case Region.Spawn:
                    return 0x01;
                case Region.Regen:
                    return 0x51;
                case Region.BlocksSight:
                    return 0x52;
                case Region.Note:
                    return 0x53;
                case Region.Enemy1:
                case Region.Enemy2:                   
                case Region.Enemy3:                   
                case Region.Enemy4:                  
                case Region.Enemy5:                  
                case Region.Enemy6:
                    return 0x59;
                case Region.Decoration1:
                case Region.Decoration2:
                case Region.Decoration3:
                case Region.Decoration4:
                case Region.Decoration5:
                case Region.Decoration6:
                    return 0x5f;
                case Region.Trigger1:
                    break;
                case Region.Callback1:
                    break;
                case Region.Trigger2:
                    break;
                case Region.Callback2:
                    break;
                case Region.Trigger3:
                    break;
                case Region.Callback3:
                    break;
                case Region.VaultChest:
                    break;
                case Region.GiftChest:
                    break;
                case Region.VaultPortal:
                    break;
                case Region.RealmPortal:
                    break;
                case Region.GuildPortal:
                    break;
                case Region.Store1:
                    return 0x03;
                case Region.Store2:
                    return 0x04;
                case Region.Store3:
                    return 0x05;
                case Region.Store4:
                    return 0x06;
                case Region.Store5:
                    return 0x07;
                case Region.Store6:
                    return 0x08;
                case Region.Vault:
                    break;
                case Region.Loot:
                    return 0x0a;
                case Region.Defender:
                    return 0x0b;
                case Region.Defender1:
                    return 0xbf3;
                case Region.Defender2:
                    return 0xbf2;
                case Region.Defender3:
                    return 0xbf1;
                case Region.Hallway:
                    return 0x0c;
                case Region.Enemy:
                    return 0x0d;
                case Region.Hallway1:
                    return 0x0e;
                case Region.Hallway2:
                    return 0x0f;
                case Region.Hallway3:
                    return 0x10;
                case Region.Store7:
                    return 0x11;
                case Region.Store8:
                    return 0x12;
                case Region.Store9:
                    return 0x13;
                case Region.Store10:
                    break;
                case Region.Store11:
                    break;
                case Region.Store12:
                    break;
                case Region.Store13:
                    break;
                case Region.Store14:
                    break;
                case Region.Store15:
                    break;
                case Region.Store16:
                    break;
                case Region.Store17:
                    break;
                case Region.Store18:
                    break;
                case Region.Store19:
                    break;
                case Region.Store20:
                    break;
                case Region.Store21:
                    break;
                case Region.Store22:
                    break;
                case Region.Store23:
                    break;
                case Region.Store24:
                    break;
                case Region.PetRegion:
                    break;
                case Region.OutsideArena:
                    break;
                case Region.ItemSpawnPoint:
                    break;
                case Region.ArenaCentralSpawn:
                    break;
                case Region.ArenaEdgeSpawn:
                    break;
                case Region.Store25:
                    break;
                case Region.Store26:
                    break;
                case Region.Store27:
                    break;
                case Region.Store28:
                    break;
                case Region.Store29:
                    break;
                case Region.Store30:
                    break;
                case Region.Store31:
                    break;
                case Region.Store32:
                    break;
                case Region.Store33:
                    break;
                case Region.Store34:
                    break;
                case Region.Store35:
                    break;
                case Region.Store36:
                    break;
                case Region.Store37:
                    break;
                case Region.Store38:
                    break;
                case Region.Store39:
                    break;
                case Region.Store40:
                    break;
                case Region.MarketPortal:
                    break;
                case Region.BazaarPortal:
                    break;
                case Region.QuestMonsterRegion:
                    break;
            }
            return 0x01;
        }
    }
}

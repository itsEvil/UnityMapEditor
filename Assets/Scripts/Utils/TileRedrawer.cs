using Models.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class Square
    {
        public ushort Type;
        public TileDesc Desc => AssetLibrary.Type2TileDesc[Type];
    }
    public static class TileRedrawer
    {
        private static readonly Dictionary<object[], Sprite> _Cache = new Dictionary<object[], Sprite>(new CacheComparer());
        private static readonly Dictionary<ushort, Sprite> _TileCache = new Dictionary<ushort, Sprite>(new TileCacheComparer());

        private static readonly Rect TextureRect = new Rect(0, 0, 8, 8);
        private static readonly RectInt Rect0 = new RectInt(0, 0, 4, 4);
        private static readonly RectInt Rect1 = new RectInt(4, 0, 4, 4);
        private static readonly RectInt Rect2 = new RectInt(0, 4, 4, 4);
        private static readonly RectInt Rect3 = new RectInt(4, 4, 4, 4);

        private static readonly Vector2Int Point0 = new Vector2Int(0, 0);
        private static readonly Vector2Int Point1 = new Vector2Int(4, 0);
        private static readonly Vector2Int Point2 = new Vector2Int(0, 4);
        private static readonly Vector2Int Point3 = new Vector2Int(4, 4);

        private static readonly List<List<List<Sprite>>> _MaskLists = GetMasks();
        private const int _INNER = 0;
        private const int _SIDE0 = 1;
        private const int _SIDE1 = 2;
        private const int _OUTER = 3;
        private const int _INNER_P1 = 4;
        private const int _INNER_P2 = 5;
        private static void RedrawRect(Texture2D texture, RectInt rect, List<List<Sprite>> masks, int b,
            int n0, int n1, int n2)
        {
            Sprite mask;
            Sprite blend;
            if (b == n0 && b == n2)
            {
                mask = masks[_OUTER].Random();
                blend = AssetLibrary.GetTileImage(n1);
            }
            else if (b != n0 && b != n2)
            {
                if (n0 != n2)
                {
                    var n0Image = SpriteUtils.CreateTexture(AssetLibrary.GetTileImage(n0));
                    var n2Image = SpriteUtils.CreateTexture(AssetLibrary.GetTileImage(n2));
                    texture.CopyPixels(n0Image, rect, masks[_INNER_P2].Random());
                    texture.CopyPixels(n2Image, rect, masks[_INNER_P1].Random());
                    return;
                }

                mask = masks[_INNER].Random();
                blend = AssetLibrary.GetTileImage(n0);
            }
            else if (b != n0)
            {
                mask = masks[_SIDE0].Random();
                blend = AssetLibrary.GetTileImage(n0);
            }
            else
            {
                mask = masks[_SIDE1].Random();
                blend = AssetLibrary.GetTileImage(n2);
            }

            var blendTex = SpriteUtils.CreateTexture(blend);
            texture.CopyPixels(blendTex, rect, mask);
        }

        private static List<List<List<Sprite>>> GetMasks()
        {
            var list = new List<List<List<Sprite>>>();
            AddMasks(list, AssetLibrary.GetImageSet("inner_mask"), AssetLibrary.GetImageSet("sides_mask"),
                AssetLibrary.GetImageSet("outer_mask"), AssetLibrary.GetImageSet("innerP1_mask"),
                AssetLibrary.GetImageSet("innerP2_mask"));
            return list;
        }

        private static void AddMasks(List<List<List<Sprite>>> list, List<Sprite> inner, List<Sprite> side, List<Sprite> outer,
            List<Sprite> innerP1, List<Sprite> innerP2)
        {
            foreach (var i in new[] { -1, 0, 2, 1 })
            {
                list.Add(new List<List<Sprite>>()
                {
                    RotateImageSet(inner, i - 1), RotateImageSet(side, i - 1), RotateImageSet(side, i),
                    RotateImageSet(outer, i - 1), RotateImageSet(innerP1, i - 1), RotateImageSet(innerP2, i - 1)
                });
            }
        }

        private static List<Sprite> RotateImageSet(List<Sprite> sprites, int clockwiseTurns)
        {
            var rotatedSprites = new List<Sprite>();
            foreach (var sprite in sprites)
            {
                rotatedSprites.Add(SpriteUtils.Rotate(sprite, clockwiseTurns));
            }

            return rotatedSprites;
        }

        private static Sprite DrawEdges(object[] sig)
        {
            var orig = AssetLibrary.GetTileImage((int) sig[4]);
            var texture = SpriteUtils.CreateTexture(orig);
            var desc = AssetLibrary.GetTileDesc((int) sig[4]);
            var edges = desc.GetEdges();
            var innerCorners = desc.GetInnerCorners();
            for (var i = 1; i < 8; i += 2)
            {
                if (!(bool) sig[i])
                {
                    texture.SetPixels32(edges[i].texture.GetPixels32());
                }
            }

            var s0 = (bool) sig[0];
            var s1 = (bool) sig[1];
            var s2 = (bool) sig[2];
            var s3 = (bool) sig[3];
            var s5 = (bool) sig[5];
            var s6 = (bool) sig[6];
            var s7 = (bool) sig[7];
            var s8 = (bool) sig[8];
            if (edges[0] != null)
            {
                if (s3 && s1 && !s0)
                {
                    texture.SetPixels32(edges[0].texture.GetPixels32());
                }
                if (s1 && s5 && !s2)
                {
                    texture.SetPixels32(edges[2].texture.GetPixels32());
                }
                if (s5 && s7 && !s8)
                {
                    texture.SetPixels32(edges[8].texture.GetPixels32());
                }
                if (s3 && s7 && !s6)
                {
                    texture.SetPixels32(edges[6].texture.GetPixels32());
                }
            }

            if (innerCorners != null)
            {
                if (!s3 && !s1)
                {
                    texture.SetPixels32(innerCorners[0].texture.GetPixels32());
                }
                if (!s1 && !s5)
                {
                    texture.SetPixels32(innerCorners[2].texture.GetPixels32());
                }
                if (!s5 && !s7)
                {
                    texture.SetPixels32(innerCorners[8].texture.GetPixels32());
                }
                if (!s7 && !s3)
                {
                    texture.SetPixels32(innerCorners[6].texture.GetPixels32());
                }
            }
            
            texture.Apply();
            return Sprite.Create(texture, TextureRect, SpriteUtils.Pivot, SpriteUtils.PIXELS_PER_UNIT);
        }

        private static Sprite BuildComposite(object[] sig)
        {
            var texture = new Texture2D((int) TextureRect.width, (int) TextureRect.height);
            texture.filterMode = FilterMode.Point;
            var s0 = (int) sig[0];
            var s1 = (int) sig[1];
            var s2 = (int) sig[2];
            var s3 = (int) sig[3];
            if (s0 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s0);
                var pixels = neighbor.texture.GetPixels(Point0.x, Point0.y, Rect0.width, Rect0.height);
                texture.SetPixels(Point0.x, Point0.y, Rect0.width, Rect0.height, pixels);
            }
            if (s1 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s1);
                var pixels = neighbor.texture.GetPixels(Point1.x, Point1.y, Rect1.width, Rect1.height);
                texture.SetPixels(Point1.x, Point1.y, Rect1.width, Rect1.height, pixels);
            }
            if (s2 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s2);
                var pixels = neighbor.texture.GetPixels(Point2.x, Point2.y, Rect2.width, Rect2.height);
                texture.SetPixels(Point2.x, Point2.y, Rect2.width, Rect2.height, pixels);
            }
            if (s3 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s3);
                var pixels = neighbor.texture.GetPixels(Point3.x, Point3.y, Rect3.width, Rect3.height);
                texture.SetPixels(Point3.x, Point3.y, Rect3.width, Rect3.height, pixels);
            }
            texture.Apply();
            return Sprite.Create(texture, TextureRect, SpriteUtils.Pivot, SpriteUtils.PIXELS_PER_UNIT);
        }
        private class CacheComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode(object[] obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }        
        private class TileCacheComparer : IEqualityComparer<ushort>
        {
            public bool Equals(ushort x, ushort y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode(ushort obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }
}
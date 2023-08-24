using System.Collections.Generic;

public partial class JSMap : Map
{
    private class Entry
    {
        public int Type;
        public List<Position> Positions;
    }

    private struct mapData
    {
        public int height { get; set; }
        public int width { get; set; }
        public entires entires { get; set; }
    }
    private struct entires
    {
        public entry[] tiles { get; set; }
        public entry[] objects { get; set; }
        public entry[] regions { get; set; } 
    }
    private struct entry
    {
        public int type { get; set; }
        public string data { get; set; }
        //public Position[] positions { get; set; }
    }
    private struct Position 
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
    //old
    private struct Encode_json_dat
    {
        public int height { get; set; }
        public int width { get; set; }
        public loc[] dict { get; set; }
        public string data { get; set; }
    }

    private struct json_dat
    {
        public byte[] data { get; set; }
        public loc[] dict { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    private struct loc
    {
        public string ground { get; set; }
        public obj[] objs { get; set; }
        public obj[] regions { get; set; }
    }

    private struct obj
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public override string ToString()
    {
        return $"[JSMap] [{Width}:{Height}] [{Tiles.GetLength(0)}:{Tiles.GetLength(1)}]";
    }
}
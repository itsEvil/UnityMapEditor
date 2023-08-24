using UnityEngine;

namespace Models.Static
{
    public class RunningAnimation
    {
        public AnimationData Data;
        public int Start;
        public int FrameId;
        public int FrameStart;
        public Sprite Texture;
        public RunningAnimation(AnimationData data, int start)
        {
            Data = data;
            Start = start;
            FrameId = 0;
            FrameStart = start;
            Texture = null;
        }
        public Sprite GetTexture(int time)
        {
            FrameData frame = Data.Frames[FrameId];
            while(time - FrameStart > frame.Time)
            {
                if(FrameId >= Data.Frames.Count - 1)
                {
                    return null;
                }
                FrameStart += frame.Time;
                FrameId++;
                frame = Data.Frames[FrameId];
                Texture = null;
            }
            if(Texture == null)
            {
                Texture = frame.TextureData.GetTexture(0);
            }
            return Texture;
        }

    }
}

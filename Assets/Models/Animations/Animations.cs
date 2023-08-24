using System.Collections.Generic;
using UnityEngine;

namespace Models.Static
{
    public class Animations
    {
        public AnimationsData Data;
        public List<int> NextRun;
        public RunningAnimation Running;
        public Animations(AnimationsData data)
        {
            Data = data;
        }
        public Sprite GetTexture(int time)
        {
            AnimationData data = null;
            Sprite texture = null;
            int start = 0;

            if(NextRun == null)
            {
                NextRun = new List<int>();
                foreach(var animationData in Data.Animations)
                {
                    data = animationData;
                    NextRun.Add(data.GetLastRun(time));
                }
            }
            if(Running != null)
            {
                texture = Running.GetTexture(time);
                if(texture != null) 
                {
                    return texture;
                }
                Running = null;
            }

            for(int i = 0;i < NextRun.Count; i++) 
            {
                //Debug.Log($"[Animation] {time} {NextRun[i]} {time > NextRun[i]}");
                if(time > NextRun[i]) 
                {
                    start = NextRun[i];
                    data = Data.Animations[i];
                    NextRun[i] = data.GetNextRun(time);
                    
                    if(data.Probablity != 1 && Random.value > data.Probablity)
                    {
                        Running = new RunningAnimation(data, start);
                        return Running.GetTexture(time);
                    }
                    else
                    {
                        Running = new RunningAnimation(data, start);
                        return Running.GetTexture(time);
                    }
                }
            }
            return null;

        }
    }
}
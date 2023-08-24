using Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Editor : MonoBehaviour
{
    private void OnEnable()
    {
        CalculatedSightCircle = new HashSet<IntPoint>();
        InitSightCircle();
        InitSightRays();
    }

    private static readonly IntPoint[] SurroundingPoints = new IntPoint[]
    {
         new IntPoint(1, 0),
         new IntPoint(1, 1),
         new IntPoint(0, 1),
         new IntPoint(-1, 1),
         new IntPoint(-1, 0),
         new IntPoint(-1, -1),
         new IntPoint(0, -1),
         new IntPoint(1, -1)
    };

    private static HashSet<IntPoint>[] SightRays;
    private static HashSet<IntPoint> SightCircle;
    public const int SightRadius = 30;
    private const float StartAngle = 0;
    private const float EndAngle = (float)(2 * Math.PI);
    private const float RayStepSize = .05f;
    private const float AngleStepSize = 2.30f / (SightRadius * 2);

    public HashSet<IntPoint> CalculatedSightCircle;
    public int[,] LoadedTiles;
    public static void InitSightCircle()
    {
        SightCircle = new HashSet<IntPoint>();
        for (var x = -SightRadius; x <= SightRadius; x++)
            for (var y = -SightRadius; y <= SightRadius; y++)
                if (x * x + y * y <= SightRadius * SightRadius)
                    SightCircle.Add(new IntPoint(x, y));

        Debug.Log("Created sight circle");
    }
    public static void InitSightRays()
    {
        var sightRays = new List<HashSet<IntPoint>>();

        var currentAngle = StartAngle;
        while (currentAngle < EndAngle)
        {
            var ray = new HashSet<IntPoint>();
            var dist = RayStepSize;
            while (dist < SightRadius + 25)
            {
                var point = new IntPoint(
                    (int)(dist * Math.Cos(currentAngle)),
                    (int)(dist * Math.Sin(currentAngle)));

                if (SightCircle.Contains(point))
                    ray.Add(point);
                dist += RayStepSize;
            }
            sightRays.Add(ray);
            currentAngle += AngleStepSize;
        }

        SightRays = sightRays.ToArray();
    }
}

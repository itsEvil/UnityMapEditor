using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class Helpers
{
    public static Camera camera;
    public static Camera Camera
    {
        get
        {
            if (camera == null) camera = Camera.main;
            return camera;
        }

    }

    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUi()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }
}

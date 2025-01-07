using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckUI
{
    public static bool IsPointerOverUIObject(string tag = "")
    {
        if (EventSystem.current == null) return false;
        
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if (results.Count == 0) return false;

        if (tag == "") return true;
        
        foreach (var r in results)
            if (r.gameObject.tag == tag) return true;
        return false;
    }
}

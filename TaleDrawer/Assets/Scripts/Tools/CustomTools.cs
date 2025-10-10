using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public static class CustomTools
{
    public static Vector2 ToVector2(Vector3 originalVector)
    {
        return new Vector2(originalVector.x, originalVector.y);
    }

    public static CustomNode GetClosestNode(Vector2 nextPos, List<CustomNode> nodes)
    {
        return nodes.OrderBy(x => Vector2.Distance(x.transform.position, nextPos)).First();

    }

    public static bool IsTouchOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}

using UnityEngine;

public static class CustomTools
{
    public static Vector2 ToVector2(Vector3 originalVector)
    {
        return new Vector2(originalVector.x, originalVector.y);
    }
}

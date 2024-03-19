using UnityEngine;

public static class Utilities
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static bool Approximately(Vector2 v1, Vector2 v2)
    {
        return Vector2.Distance(v1, v2) < 0.001f;
    }
}

using UnityEngine;

public static class Utilities
{
    public static float PlanetRadius = 2.5f;
    public static float InnerOrbit = 3f;
    public static float OuterOrbit = 4.25f;

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static bool Approximately(Vector2 v1, Vector2 v2)
    {
        return Vector2.Distance(v1, v2) < 0.001f;
    }

    public static bool LayerMaskContainsLayer(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | 1 << layer);
    }
}

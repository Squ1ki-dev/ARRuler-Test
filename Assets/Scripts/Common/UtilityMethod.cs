using UnityEngine;

public static class UtilityMethod
{
    public static Vector3 AdjustScaleBasedOnDistance(float distance, DistanceScaleRange range)
    {
        distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);
        
        float scale = Mathf.Lerp(range.minScale, range.maxScale, (distance - range.minDistance) / (range.maxDistance - range.minDistance));
        return new Vector3(scale, scale, scale);
    }

    public static float AdjustValueBasedOnDistance(float distance, DistanceScaleRange range)
    {
        distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);

        float scale = Mathf.Lerp(range.minScale, range.maxScale, (distance - range.minDistance) / (range.maxDistance - range.minDistance));
        return scale;
    }
}

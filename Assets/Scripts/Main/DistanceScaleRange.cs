public struct DistanceScaleRange
{
    public float minDistance;
    public float maxDistance;
    public float minScale;
    public float maxScale;

    public DistanceScaleRange(float minDistance, float maxDistance, float minScale, float maxScale)
    {
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        this.minScale = minScale;
        this.maxScale = maxScale;
    }
}

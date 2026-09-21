namespace Core.Gameplay.Track
{
    public interface ITrackProvider
    {
        float StartCoordinate { get; }
        float FinishCoordinate { get; }
        float RunLength { get; }
    }
}

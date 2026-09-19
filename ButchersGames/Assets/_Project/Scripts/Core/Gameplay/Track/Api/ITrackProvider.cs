namespace Core.Gameplay.Track
{
    public interface ITrackProvider
    {
        float Length { get; }
        float FinishDistance { get; }
    }
}

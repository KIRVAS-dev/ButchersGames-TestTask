namespace Core.Gameplay.LevelProgression
{
    public interface ILevelProvider
    {
        int LevelCount { get; }
        bool IsRandomized { get; }
    }
}

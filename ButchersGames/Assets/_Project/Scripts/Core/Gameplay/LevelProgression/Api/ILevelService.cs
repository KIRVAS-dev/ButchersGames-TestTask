namespace Core.Gameplay.LevelProgression
{
    public interface ILevelService
    {
        int CurrentLevelNumber { get; }
        void LoadCurrentLevel();
        void LoadNextLevel();
    }
}

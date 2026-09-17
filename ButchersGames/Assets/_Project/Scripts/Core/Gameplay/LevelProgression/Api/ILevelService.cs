namespace Core.Gameplay.LevelProgression
{
    public interface ILevelService
    {
        int CurrentLevelNumber { get; }
        int CompletedLevelCount { get; }

        void SelectCurrentLevel();
        void RestartLevel();
        void ProceedToNextLevel();
    }
}

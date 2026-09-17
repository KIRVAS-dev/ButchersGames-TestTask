namespace Core.Gameplay.GameFlow
{
    public interface IGameFlowService
    {
        GameFlowState State { get; }

        void StartGame();
        void RetryLevel();
        void ProceedToNextLevel();
    }
}

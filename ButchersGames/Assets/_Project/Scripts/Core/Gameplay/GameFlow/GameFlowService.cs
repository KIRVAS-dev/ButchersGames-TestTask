using Core.Gameplay.LevelProgression;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameFlowService : IGameFlowService
    {
        private readonly ILevelService _levelService;
        private readonly IGameStateMachine _gameStateMachine;

        public GameFlowService(ILevelService levelService, IGameStateMachine gameStateMachine)
        {
            _levelService = levelService;
            _gameStateMachine = gameStateMachine;
        }

        public void PrepareGame()
        {
            _gameStateMachine.EnterState(GameState.Tutorial);
            _levelService.LoadCurrentLevel();
        }

        public void StartGame()
        {
            _gameStateMachine.EnterState(GameState.Run);
        }

        public void FinishGame(GameState result)
        {
            _gameStateMachine.EnterState(result);
        }

        public void GoToNextGame()
        {
            _gameStateMachine.EnterState(GameState.Tutorial);
            _levelService.LoadNextLevel();
        }
    }
}

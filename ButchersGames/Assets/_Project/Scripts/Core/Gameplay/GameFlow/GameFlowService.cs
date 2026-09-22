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

        void IGameFlowService.PrepareGame()
        {
            _gameStateMachine.EnterState(GameState.Tutorial);
            _levelService.LoadCurrentLevel();
        }

        void IGameFlowService.StartGame()
        {
            _gameStateMachine.EnterState(GameState.Run);
        }

        void IGameFlowService.FinishGame(GameState result)
        {
            _gameStateMachine.EnterState(result);
        }

        void IGameFlowService.GoToNextGame()
        {
            _gameStateMachine.EnterState(GameState.Tutorial);
            _levelService.LoadNextLevel();
        }
    }
}

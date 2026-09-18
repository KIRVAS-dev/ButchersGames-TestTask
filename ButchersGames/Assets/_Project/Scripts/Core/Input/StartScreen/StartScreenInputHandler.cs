using Core.Gameplay.GameFlow;
using IStartScreenInput = global::Input.IStartScreenInput;

namespace Core.Input.StartScreen
{
    public sealed class StartScreenInputHandler
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly IStartScreenInput _input;

        public StartScreenInputHandler(IGameFlowService gameFlowService, IStartScreenInput input)
        {
            _gameFlowService = gameFlowService;
            _input = input;
        }

        public void StartListening()
        {
            _input.StartTrigger.Triggered += OnStartTriggered;
        }

        public void StopListening()
        {
            _input.StartTrigger.Triggered -= OnStartTriggered;
        }

        private void OnStartTriggered()
        {
            _gameFlowService.StartGame();
        }
    }
}

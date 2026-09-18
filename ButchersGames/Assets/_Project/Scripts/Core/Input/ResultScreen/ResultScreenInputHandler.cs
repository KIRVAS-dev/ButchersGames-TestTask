using Core.Gameplay.GameFlow;
using IResultScreenInput = global::Input.IResultScreenInput;

namespace Core.Input.ResultScreen
{
    public sealed class ResultScreenInputHandler
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly IResultScreenInput _input;

        public ResultScreenInputHandler(IGameFlowService gameFlowService, IResultScreenInput input)
        {
            _gameFlowService = gameFlowService;
            _input = input;
        }

        public void StartListening()
        {
            _input.RetryTrigger.Triggered += OnRetryTriggered;
            _input.NextTrigger.Triggered += OnNextTriggered;
        }

        public void StopListening()
        {
            _input.RetryTrigger.Triggered -= OnRetryTriggered;
            _input.NextTrigger.Triggered -= OnNextTriggered;
        }

        private void OnRetryTriggered()
        {
            _gameFlowService.RetryLevel();
        }

        private void OnNextTriggered()
        {
            _gameFlowService.ProceedToNextLevel();
        }
    }
}

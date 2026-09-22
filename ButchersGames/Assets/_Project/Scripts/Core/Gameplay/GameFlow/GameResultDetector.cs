using System;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.Track;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameResultDetector : ISubscriptionLifecycle
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IWealthMeterService _wealthMeter;
        private readonly ITrackProvider _trackProvider;
        private readonly RunnerMovementModel _runnerMovementModel;

        private IDisposable _runnerCurrentCoordinateSubscription;

        public GameResultDetector(
            IGameFlowService gameFlowService,
            IGameStateMachine gameStateMachine,
            IWealthMeterService wealthMeter,
            ITrackProvider trackProvider,
            RunnerMovementModel runnerMovementModel)
        {
            _gameFlowService = gameFlowService;
            _gameStateMachine = gameStateMachine;
            _wealthMeter = wealthMeter;
            _trackProvider = trackProvider;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeter.Depleted += OnWealthDepleted;

            _runnerCurrentCoordinateSubscription =
                _runnerMovementModel.CurrentRunnerCoordinate.Subscribe(OnCurrentRunnerCoordinateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeter.Depleted -= OnWealthDepleted;
            _runnerCurrentCoordinateSubscription?.Dispose();
        }

        private void OnWealthDepleted()
        {
            FinishRun(GameState.Lose);
        }

        private void OnCurrentRunnerCoordinateChanged(float coordinate)
        {
            if (_gameStateMachine.State != GameState.Run)
            {
                return;
            }

            if (coordinate >= _trackProvider.FinishCoordinate)
            {
                FinishRun(GameState.Win);
            }
        }

        private void FinishRun(GameState result)
        {
            if (_gameStateMachine.State != GameState.Run)
            {
                return;
            }

            _gameFlowService.FinishGame(result);
        }
    }
}

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
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IWealthMeterEvents _wealthMeterEvents;
        private readonly ITrackProvider _trackProvider;
        private readonly IReadOnlyRunnerMovementModel _runnerMovementModel;

        private IDisposable _runnerCurrentCoordinateSubscription;

        public GameResultDetector(
            IGameFlowService gameFlowService,
            IReadOnlyGameStateModel gameStateModel,
            IWealthMeterEvents wealthMeterEvents,
            ITrackProvider trackProvider,
            IReadOnlyRunnerMovementModel runnerMovementModel)
        {
            _gameFlowService = gameFlowService;
            _gameStateModel = gameStateModel;
            _wealthMeterEvents = wealthMeterEvents;
            _trackProvider = trackProvider;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Depleted += OnWealthDepleted;

            _runnerCurrentCoordinateSubscription =
                _runnerMovementModel.CurrentRunnerCoordinate.Subscribe(OnCurrentRunnerCoordinateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Depleted -= OnWealthDepleted;
            _runnerCurrentCoordinateSubscription?.Dispose();
        }

        private void OnWealthDepleted()
        {
            FinishRun(GameState.Lose);
        }

        private void OnCurrentRunnerCoordinateChanged(float coordinate)
        {
            if (_gameStateModel.State.CurrentValue != GameState.Run)
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
            if (_gameStateModel.State.CurrentValue != GameState.Run)
            {
                return;
            }

            _gameFlowService.FinishGame(result);
        }
    }
}

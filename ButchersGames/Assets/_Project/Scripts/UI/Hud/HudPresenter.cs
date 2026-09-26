using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.Track;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;
using UnityEngine;

namespace UI.Hud
{
    public sealed class HudPresenter : ISubscriptionLifecycle
    {
        private const int SkipInitialValue = 1;

        private readonly IHudView _view;
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly ILevelProgress _levelProgress;
        private readonly ITrackProvider _trackProvider;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IReadOnlyWealthMeterModel _wealthMeterModel;
        private readonly IReadOnlyRunnerMovementModel _runnerMovementModel;

        private IDisposable _stateSubscription;
        private IDisposable _valueSubscription;
        private IDisposable _runProgressSubscription;

        public HudPresenter(
            IHudView view,
            ILevelLoaderEvents levelLoaderEvents,
            ILevelProgress levelProgress,
            ITrackProvider trackProvider,
            IReadOnlyGameStateModel gameStateModel,
            IReadOnlyWealthMeterModel wealthMeterModel,
            IReadOnlyRunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _levelLoaderEvents = levelLoaderEvents;
            _levelProgress = levelProgress;
            _trackProvider = trackProvider;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
            _valueSubscription = _wealthMeterModel.WealthPoints.Subscribe(OnValueChanged);

            _runProgressSubscription = _runnerMovementModel
               .CurrentRunnerCoordinate
               .Skip(SkipInitialValue)
               .Subscribe(OnCurrentCoordinateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;
            _stateSubscription?.Dispose();
            _valueSubscription?.Dispose();
            _runProgressSubscription?.Dispose();
        }

        private void OnLevelLoaded()
        {
            _view.SetLevelNumber(_levelProgress.CurrentLevelNumber);
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Run:
                    _view.Show();
                    break;

                case GameState.Tutorial:
                case GameState.Win:
                case GameState.Lose:
                    _view.Hide();
                    break;

                default:
                    throw new UnhandledHudStateException(state);
            }
        }

        private void OnValueChanged(int value)
        {
            _view.SetMoneyAmount(value);
        }

        private void OnCurrentCoordinateChanged(float coordinate)
        {
            float runProgress = (coordinate - _trackProvider.StartCoordinate) / _trackProvider.RunLength;

            _view.SetRunProgressFillBar(Mathf.Clamp01(runProgress));
        }
    }
}

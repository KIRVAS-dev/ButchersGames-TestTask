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
        private readonly ILevelLoader _levelLoader;
        private readonly ILevelService _levelService;
        private readonly ITrackProvider _trackProvider;
        private readonly GameStateModel _gameStateModel;
        private readonly WealthMeterModel _wealthMeterModel;
        private readonly RunnerMovementModel _runnerMovementModel;

        private IDisposable _stateSubscription;
        private IDisposable _valueSubscription;
        private IDisposable _runProgressSubscription;

        public HudPresenter(
            IHudView view,
            ILevelLoader levelLoader,
            ILevelService levelService,
            ITrackProvider trackProvider,
            GameStateModel gameStateModel,
            WealthMeterModel wealthMeterModel,
            RunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _levelLoader = levelLoader;
            _levelService = levelService;
            _trackProvider = trackProvider;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
            _valueSubscription = _wealthMeterModel.WealthPoints.Subscribe(OnValueChanged);

            _runProgressSubscription = _runnerMovementModel
               .CurrentRunnerCoordinate
               .Skip(SkipInitialValue)
               .Subscribe(OnCurrentCoordinateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;
            _stateSubscription?.Dispose();
            _valueSubscription?.Dispose();
            _runProgressSubscription?.Dispose();
        }

        private void OnLevelLoaded()
        {
            _view.SetLevelNumber(_levelService.CurrentLevelNumber);
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Run)
            {
                _view.Show();
            }
            else
            {
                _view.Hide();
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

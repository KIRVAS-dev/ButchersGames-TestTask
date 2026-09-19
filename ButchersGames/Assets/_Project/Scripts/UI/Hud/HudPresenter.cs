using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using R3;
using UnityEngine;

namespace UI.Hud
{
    public sealed class HudPresenter
    {
        private readonly IHudView _view;
        private readonly ILevelLoader _levelLoader;
        private readonly ILevelService _levelService;
        private readonly IWealthMeterSettings _wealthMeterSettings;
        private readonly GameStateModel _gameStateModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;
        private IDisposable _valueSubscription;
        private IDisposable _stageSubscription;

        public HudPresenter(
            IHudView view,
            ILevelLoader levelLoader,
            ILevelService levelService,
            IWealthMeterSettings wealthMeterSettings,
            GameStateModel gameStateModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _levelLoader = levelLoader;
            _levelService = levelService;
            _wealthMeterSettings = wealthMeterSettings;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
        }

        public void StartListening()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
            _valueSubscription = _wealthMeterModel.WealthPoints.Subscribe(OnValueChanged);
            _stageSubscription = _wealthMeterModel.Stage.Subscribe(_view.SetWealthStage);
        }

        public void StopListening()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;
            _stateSubscription?.Dispose();
            _valueSubscription?.Dispose();
            _stageSubscription?.Dispose();
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
            int maxValue = _wealthMeterSettings.MillionaireThreshold;

            float wealthProgress = (float)value / maxValue;
            float normalizedFill = Mathf.Clamp01(wealthProgress);

            _view.SetMoneyAmount(value);
            _view.SetWealthFillBar(normalizedFill);
        }
    }
}

using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;
using UnityEngine;

namespace UI.WealthIndicator
{
    public sealed class WealthIndicatorPresenter : ISubscriptionLifecycle
    {
        private readonly IWealthIndicatorView _view;
        private readonly IWealthMeterSettings _wealthMeterSettings;
        private readonly GameStateModel _gameStateModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;
        private IDisposable _valueSubscription;
        private IDisposable _stageSubscription;

        public WealthIndicatorPresenter(
            IWealthIndicatorView view,
            IWealthMeterSettings wealthMeterSettings,
            GameStateModel gameStateModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _wealthMeterSettings = wealthMeterSettings;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
            _valueSubscription = _wealthMeterModel.WealthPoints.Subscribe(OnValueChanged);
            _stageSubscription = _wealthMeterModel.Stage.Subscribe(_view.SetStage);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _stateSubscription?.Dispose();
            _valueSubscription?.Dispose();
            _stageSubscription?.Dispose();
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
            int maxValue = _wealthMeterSettings.RichThreshold;
            float wealthProgress = (float)value / maxValue;

            _view.SetFillBar(Mathf.Clamp01(wealthProgress));
        }
    }
}

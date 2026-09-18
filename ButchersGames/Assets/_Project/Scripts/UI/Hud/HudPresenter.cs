using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using R3;
using UnityEngine;

namespace UI.Hud
{
    public sealed class HudPresenter
    {
        private readonly IHudView _view;
        private readonly IWealthMeterSettings _wealthMeterSettings;
        private readonly GameFlowModel _gameFlowModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;
        private IDisposable _valueSubscription;

        public HudPresenter(
            IHudView view,
            IWealthMeterSettings wealthMeterSettings,
            GameFlowModel gameFlowModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _wealthMeterSettings = wealthMeterSettings;
            _gameFlowModel = gameFlowModel;
            _wealthMeterModel = wealthMeterModel;
        }

        public void StartListening()
        {
            _stateSubscription = _gameFlowModel.State.Subscribe(OnStateChanged);
            _valueSubscription = _wealthMeterModel.Value.Subscribe(OnValueChanged);
        }

        public void StopListening()
        {
            _stateSubscription?.Dispose();
            _valueSubscription?.Dispose();
        }

        private void OnStateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Playing)
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

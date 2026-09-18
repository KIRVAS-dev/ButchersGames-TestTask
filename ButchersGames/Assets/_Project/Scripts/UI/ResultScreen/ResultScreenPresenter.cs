using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using R3;

namespace UI.ResultScreen
{
    public sealed class ResultScreenPresenter
    {
        private readonly IResultScreenView _view;
        private readonly GameFlowModel _gameFlowModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;

        public ResultScreenPresenter(
            IResultScreenView view,
            GameFlowModel gameFlowModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _gameFlowModel = gameFlowModel;
            _wealthMeterModel = wealthMeterModel;
        }

        public void StartListening()
        {
            _stateSubscription = _gameFlowModel.State.Subscribe(OnStateChanged);
        }

        public void StopListening()
        {
            _stateSubscription?.Dispose();
        }

        private void OnStateChanged(GameFlowState state)
        {
            switch (state)
            {
                case GameFlowState.Win:
                    _view.SetWinResult();
                    ShowResult();
                    break;

                case GameFlowState.Lose:
                    _view.SetLoseResult();
                    ShowResult();
                    break;

                case GameFlowState.WaitingToStart:
                case GameFlowState.Playing:
                    _view.Hide();
                    break;

                default:
                    throw new UnhandledResultScreenStateException(state);
            }
        }

        private void ShowResult()
        {
            _view.SetMoneyAmount(_wealthMeterModel.Value.CurrentValue);
            _view.Show();
        }
    }
}

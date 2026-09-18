using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using R3;
using UnityEngine;

namespace UI.ResultScreen
{
    public sealed class ResultScreenPresenter
    {
        private readonly IResultScreenView _view;
        private readonly IGameFlowService _gameFlowService;
        private readonly GameFlowModel _gameFlowModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;

        public ResultScreenPresenter(
            IResultScreenView view,
            IGameFlowService gameFlowService,
            GameFlowModel gameFlowModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _gameFlowService = gameFlowService;
            _gameFlowModel = gameFlowModel;
            _wealthMeterModel = wealthMeterModel;
        }

        public void StartListening()
        {
            _view.RetryClicked += OnRetryClicked;
            _view.NextClicked += OnNextClicked;
            _stateSubscription = _gameFlowModel.State.Subscribe(OnStateChanged);
        }

        public void StopListening()
        {
            _view.RetryClicked -= OnRetryClicked;
            _view.NextClicked -= OnNextClicked;
            _stateSubscription?.Dispose();
        }

        private void OnRetryClicked()
        {
            _gameFlowService.RetryLevel();
        }

        private void OnNextClicked()
        {
            _gameFlowService.ProceedToNextLevel();
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
            int moneyAmount = Mathf.Max(0, _wealthMeterModel.Value.CurrentValue);

            _view.SetMoneyAmount(moneyAmount);
            _view.Show();
        }
    }
}

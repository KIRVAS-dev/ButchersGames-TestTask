using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace UI.ResultScreen
{
    public sealed class ResultScreenPresenter : ISubscriptionLifecycle
    {
        private readonly IResultScreenView _view;
        private readonly IGameFlowService _gameFlowService;
        private readonly ILevelService _levelService;
        private readonly IFeedbackPerformer _feedbackPerformer;
        private readonly GameStateModel _gameStateModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stateSubscription;

        public ResultScreenPresenter(
            IResultScreenView view,
            IGameFlowService gameFlowService,
            ILevelService levelService,
            IFeedbackPerformer feedbackPerformer,
            GameStateModel gameStateModel,
            WealthMeterModel wealthMeterModel)
        {
            _view = view;
            _gameFlowService = gameFlowService;
            _levelService = levelService;
            _feedbackPerformer = feedbackPerformer;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _view.RetryClicked += OnRetryClicked;
            _view.NextClicked += OnNextClicked;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _view.RetryClicked -= OnRetryClicked;
            _view.NextClicked -= OnNextClicked;
            _stateSubscription?.Dispose();
        }

        private void OnRetryClicked()
        {
            _feedbackPerformer.Play(FeedbackType.ButtonClick);
            _gameFlowService.PrepareGame();
        }

        private void OnNextClicked()
        {
            _feedbackPerformer.Play(FeedbackType.ButtonClick);
            _gameFlowService.GoToNextGame();
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Win:
                    _view.SetLevelNumber(_levelService.CurrentLevelNumber);
                    _view.SetWinResult();
                    ShowResult();
                    break;

                case GameState.Lose:
                    _view.SetLoseResult();
                    ShowResult();
                    break;

                case GameState.Tutorial:
                case GameState.Run:
                    _view.Hide();
                    break;

                default:
                    throw new UnhandledResultScreenStateException(state);
            }
        }

        private void ShowResult()
        {
            int moneyAmount = _wealthMeterModel.WealthPoints.CurrentValue;

            _view.SetMoneyAmount(moneyAmount);
            _view.Show();
        }
    }
}

using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace ViewComponents.Feedback
{
    public sealed class FeedbackPresenter : ISubscriptionLifecycle
    {
        private readonly IFeedbackPerformer _feedbackPerformer;
        private readonly IWealthMeterEvents _wealthMeterEvents;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IReadOnlyWealthMeterModel _wealthMeterModel;

        private IDisposable _stageSubscription;
        private IDisposable _stateSubscription;

        public FeedbackPresenter(
            IFeedbackPerformer feedbackPerformer,
            IWealthMeterEvents wealthMeterEvents,
            IReadOnlyGameStateModel gameStateModel,
            IReadOnlyWealthMeterModel wealthMeterModel)
        {
            _feedbackPerformer = feedbackPerformer;
            _wealthMeterEvents = wealthMeterEvents;
            _gameStateModel = gameStateModel;
            _wealthMeterModel = wealthMeterModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Increased += OnMoneyIncreased;
            _wealthMeterEvents.Decreased += OnMoneyDecreased;
            _stageSubscription = _wealthMeterModel.Stage.Subscribe(OnStageChanged);
            _stateSubscription = _gameStateModel.State.Subscribe(OnGameStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Increased -= OnMoneyIncreased;
            _wealthMeterEvents.Decreased -= OnMoneyDecreased;
            _stageSubscription?.Dispose();
            _stateSubscription?.Dispose();
        }

        private void OnMoneyIncreased(int amount)
        {
            _feedbackPerformer.Play(FeedbackType.MoneyGained);
        }

        private void OnMoneyDecreased(int amount)
        {
            _feedbackPerformer.Play(FeedbackType.MoneyLost);
        }

        private void OnStageChanged(WealthStage stage)
        {
            bool isRunning = _gameStateModel.State.CurrentValue == GameState.Run;

            if (isRunning)
            {
                _feedbackPerformer.Play(FeedbackType.StageChanged);
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Run:
                    _feedbackPerformer.Play(FeedbackType.GameStarted);
                    break;

                case GameState.Win:
                    _feedbackPerformer.Play(FeedbackType.Win);
                    break;

                case GameState.Lose:
                    _feedbackPerformer.Play(FeedbackType.Lose);
                    break;

                case GameState.Tutorial:
                    break;

                default:
                    throw new UnhandledFeedbackStateException(state);
            }
        }
    }
}

using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace ViewComponents.Feedback
{
    public sealed class AudioFeedbackPresenter : ISubscriptionLifecycle
    {
        private readonly IAudioFeedbackPerformer _feedbackPerformer;
        private readonly IWealthMeterEvents _wealthMeterEvents;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IReadOnlyWealthMeterModel _wealthMeterModel;

        private IDisposable _stageSubscription;
        private IDisposable _stateSubscription;

        public AudioFeedbackPresenter(
            IAudioFeedbackPerformer feedbackPerformer,
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
            _stageSubscription = _wealthMeterModel.Stage.Subscribe(OnWealthStageChanged);
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
            _feedbackPerformer.Play(AudioFeedbackType.MoneyGained);
        }

        private void OnMoneyDecreased(int amount)
        {
            _feedbackPerformer.Play(AudioFeedbackType.MoneyLost);
        }

        private void OnWealthStageChanged(WealthStage stage)
        {
            bool isRunning = _gameStateModel.State.CurrentValue == GameState.Run;

            if (isRunning)
            {
                _feedbackPerformer.Play(AudioFeedbackType.StageChanged);
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Win:
                    _feedbackPerformer.Play(AudioFeedbackType.Win);
                    break;

                case GameState.Run:
                case GameState.Lose:
                case GameState.Tutorial:
                    break;

                default:
                    throw new UnhandledFeedbackStateException(state);
            }
        }
    }
}

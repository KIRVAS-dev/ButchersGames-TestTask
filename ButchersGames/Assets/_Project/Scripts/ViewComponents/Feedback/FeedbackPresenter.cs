using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthMeter;
using R3;

namespace ViewComponents.Feedback
{
    public sealed class FeedbackPresenter
    {
        private readonly IFeedbackPerformer _feedbackPerformer;
        private readonly IWealthMeterService _wealthMeterService;
        private readonly GameFlowModel _gameFlowModel;
        private readonly WealthMeterModel _wealthMeterModel;

        private IDisposable _stageSubscription;
        private IDisposable _stateSubscription;

        public FeedbackPresenter(
            IFeedbackPerformer feedbackPerformer,
            IWealthMeterService wealthMeterService,
            GameFlowModel gameFlowModel,
            WealthMeterModel wealthMeterModel)
        {
            _feedbackPerformer = feedbackPerformer;
            _wealthMeterService = wealthMeterService;
            _gameFlowModel = gameFlowModel;
            _wealthMeterModel = wealthMeterModel;
        }

        public void StartListening()
        {
            _wealthMeterService.Increased += OnMoneyIncreased;
            _wealthMeterService.Decreased += OnMoneyDecreased;
            _stageSubscription = _wealthMeterModel.Stage.Subscribe(OnStageChanged);
            _stateSubscription = _gameFlowModel.State.Subscribe(OnGameFlowStateChanged);
        }

        public void StopListening()
        {
            _wealthMeterService.Increased -= OnMoneyIncreased;
            _wealthMeterService.Decreased -= OnMoneyDecreased;
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
            bool isPlaying = _gameFlowModel.State.CurrentValue == GameFlowState.Playing;

            if (isPlaying)
            {
                _feedbackPerformer.Play(FeedbackType.StageChanged);
            }
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            switch (state)
            {
                case GameFlowState.Playing:
                    _feedbackPerformer.Play(FeedbackType.GameStarted);
                    break;

                case GameFlowState.Win:
                    _feedbackPerformer.Play(FeedbackType.Win);
                    break;

                case GameFlowState.Lose:
                    _feedbackPerformer.Play(FeedbackType.Lose);
                    break;

                case GameFlowState.WaitingToStart:
                    break;

                default:
                    throw new UnhandledFeedbackStateException(state);
            }
        }
    }
}

using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Lifecycle;
using R3;

namespace UI.StartScreen
{
    public sealed class StartScreenPresenter : ISubscriptionLifecycle
    {
        private readonly IStartScreenView _view;
        private readonly IGameFlowService _gameFlowService;
        private readonly IFeedbackPerformer _feedbackPerformer;
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly ILevelProgress _levelProgress;
        private readonly IReadOnlyGameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public StartScreenPresenter(
            IStartScreenView view,
            IGameFlowService gameFlowService,
            IFeedbackPerformer feedbackPerformer,
            ILevelLoaderEvents levelLoaderEvents,
            ILevelProgress levelProgress,
            IReadOnlyGameStateModel gameStateModel)
        {
            _view = view;
            _gameFlowService = gameFlowService;
            _feedbackPerformer = feedbackPerformer;
            _levelLoaderEvents = levelLoaderEvents;
            _levelProgress = levelProgress;
            _gameStateModel = gameStateModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _view.StartClicked += OnStartClicked;
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _view.StartClicked -= OnStartClicked;
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;
            _stateSubscription?.Dispose();
        }

        private void OnStartClicked()
        {
            _feedbackPerformer.Play(FeedbackType.ButtonClick);
            _gameFlowService.StartGame();
        }

        private void OnLevelLoaded()
        {
            _view.SetLevelNumber(_levelProgress.CurrentLevelNumber);
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Tutorial:
                    _view.Show();
                    break;

                case GameState.Run:
                case GameState.Win:
                case GameState.Lose:
                    _view.Hide();
                    break;

                default:
                    throw new UnhandledStartScreenStateException(state);
            }
        }
    }
}

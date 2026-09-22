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
        private readonly ILevelLoader _levelLoader;
        private readonly ILevelService _levelService;
        private readonly GameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public StartScreenPresenter(
            IStartScreenView view,
            IGameFlowService gameFlowService,
            IFeedbackPerformer feedbackPerformer,
            ILevelLoader levelLoader,
            ILevelService levelService,
            GameStateModel gameStateModel)
        {
            _view = view;
            _gameFlowService = gameFlowService;
            _feedbackPerformer = feedbackPerformer;
            _levelLoader = levelLoader;
            _levelService = levelService;
            _gameStateModel = gameStateModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _view.StartClicked += OnStartClicked;
            _levelLoader.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _view.StartClicked -= OnStartClicked;
            _levelLoader.LevelLoaded -= OnLevelLoaded;
            _stateSubscription?.Dispose();
        }

        private void OnStartClicked()
        {
            _feedbackPerformer.Play(FeedbackType.ButtonClick);
            _gameFlowService.StartGame();
        }

        private void OnLevelLoaded()
        {
            _view.SetLevelNumber(_levelService.CurrentLevelNumber);
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Tutorial)
            {
                _view.Show();
            }
            else
            {
                _view.Hide();
            }
        }
    }
}

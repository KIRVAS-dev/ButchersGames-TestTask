using System;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using R3;

namespace UI.StartScreen
{
    public sealed class StartScreenPresenter
    {
        private readonly IStartScreenView _view;
        private readonly IGameFlowService _gameFlowService;
        private readonly IFeedbackPerformer _feedbackPerformer;
        private readonly ILevelProvider _levelProvider;
        private readonly ILevelService _levelService;
        private readonly GameFlowModel _gameFlowModel;

        private IDisposable _stateSubscription;

        public StartScreenPresenter(
            IStartScreenView view,
            IGameFlowService gameFlowService,
            IFeedbackPerformer feedbackPerformer,
            ILevelProvider levelProvider,
            ILevelService levelService,
            GameFlowModel gameFlowModel)
        {
            _view = view;
            _gameFlowService = gameFlowService;
            _feedbackPerformer = feedbackPerformer;
            _levelProvider = levelProvider;
            _levelService = levelService;
            _gameFlowModel = gameFlowModel;
        }

        public void StartListening()
        {
            _view.StartClicked += OnStartClicked;
            _levelProvider.LevelLoaded += OnLevelLoaded;
            _stateSubscription = _gameFlowModel.State.Subscribe(OnStateChanged);
        }

        public void StopListening()
        {
            _view.StartClicked -= OnStartClicked;
            _levelProvider.LevelLoaded -= OnLevelLoaded;
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

        private void OnStateChanged(GameFlowState state)
        {
            if (state == GameFlowState.WaitingToStart)
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

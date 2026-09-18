using System;
using Core.Gameplay.GameFlow;
using R3;

namespace UI.StartScreen
{
    public sealed class StartScreenPresenter
    {
        private readonly IStartScreenView _view;
        private readonly GameFlowModel _gameFlowModel;

        private IDisposable _stateSubscription;

        public StartScreenPresenter(IStartScreenView view, GameFlowModel gameFlowModel)
        {
            _view = view;
            _gameFlowModel = gameFlowModel;
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

using System;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using R3;

namespace ViewComponents.StartCamera
{
    public sealed class StartCameraPresenter : ISubscriptionLifecycle
    {
        private readonly IStartCameraView _view;
        private readonly IReadOnlyGameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public StartCameraPresenter(IStartCameraView view, IReadOnlyGameStateModel gameStateModel)
        {
            _view = view;
            _gameStateModel = gameStateModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _stateSubscription?.Dispose();
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Tutorial:
                    _view.ShowStart();
                    break;

                case GameState.Run:
                    _view.ShowGameplay();
                    break;

                case GameState.Win:
                case GameState.Lose:
                    break;

                default:
                    throw new UnhandledStartCameraStateException(state);
            }
        }
    }
}

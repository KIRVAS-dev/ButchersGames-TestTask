using System;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using R3;

namespace ViewComponents.CelebrationCamera
{
    public sealed class CelebrationCameraPresenter : ISubscriptionLifecycle
    {
        private readonly ICelebrationCameraView _view;
        private readonly GameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public CelebrationCameraPresenter(ICelebrationCameraView view, GameStateModel gameStateModel)
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
                case GameState.Win:
                    _view.Play();
                    break;

                case GameState.Tutorial:
                case GameState.Run:
                case GameState.Lose:
                    _view.Stop();
                    break;

                default:
                    throw new UnhandledCelebrationCameraStateException(state);
            }
        }
    }
}

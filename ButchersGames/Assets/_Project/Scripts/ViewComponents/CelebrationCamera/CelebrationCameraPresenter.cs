using System;
using Core.Gameplay.GameFlow;
using R3;

namespace ViewComponents.CelebrationCamera
{
    public sealed class CelebrationCameraPresenter
    {
        private readonly ICelebrationCameraView _view;
        private readonly GameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public CelebrationCameraPresenter(ICelebrationCameraView view, GameStateModel gameStateModel)
        {
            _view = view;
            _gameStateModel = gameStateModel;
        }

        public void StartListening()
        {
            _stateSubscription = _gameStateModel.State.Subscribe(OnStateChanged);
        }

        public void StopListening()
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

                case GameState.Run:
                    _view.Cancel();
                    break;

                case GameState.Tutorial:
                case GameState.Lose:
                    _view.Stop();
                    break;

                default:
                    throw new UnhandledCelebrationCameraStateException(state);
            }
        }
    }
}

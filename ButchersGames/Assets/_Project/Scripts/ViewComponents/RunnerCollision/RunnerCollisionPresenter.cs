using System;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using R3;

namespace ViewComponents.RunnerCollision
{
    public sealed class RunnerCollisionPresenter : ISubscriptionLifecycle
    {
        private readonly IRunnerCollisionView _view;
        private readonly IReadOnlyGameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public RunnerCollisionPresenter(IRunnerCollisionView view, IReadOnlyGameStateModel gameStateModel)
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
                case GameState.Run:
                    _view.EnableCollisions();
                    break;

                case GameState.Tutorial:
                case GameState.Win:
                case GameState.Lose:
                    _view.DisableCollisions();
                    break;

                default:
                    throw new UnhandledRunnerCollisionStateException(state);
            }
        }
    }
}

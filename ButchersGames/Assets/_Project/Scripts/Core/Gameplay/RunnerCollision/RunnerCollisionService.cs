using System;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using R3;

namespace Core.Gameplay.RunnerCollision
{
    public sealed class RunnerCollisionService : ISubscriptionLifecycle
    {
        private readonly IRunnerCollision _runnerCollision;
        private readonly IReadOnlyGameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public RunnerCollisionService(IRunnerCollision runnerCollision, IReadOnlyGameStateModel gameStateModel)
        {
            _runnerCollision = runnerCollision;
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
                    _runnerCollision.EnableCollisions();
                    break;

                case GameState.Tutorial:
                case GameState.Win:
                case GameState.Lose:
                    _runnerCollision.DisableCollisions();
                    break;

                default:
                    throw new UnhandledRunnerCollisionStateException(state);
            }
        }
    }
}

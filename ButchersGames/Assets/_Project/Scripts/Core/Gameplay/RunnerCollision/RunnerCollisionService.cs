using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerBody;
using Core.Lifecycle;
using R3;

namespace Core.Gameplay.RunnerCollision
{
    public sealed class RunnerCollisionService : ISubscriptionLifecycle
    {
        private readonly IRunnerBodyCollision _runnerBodyCollision;
        private readonly IReadOnlyGameStateModel _gameStateModel;

        private IDisposable _stateSubscription;

        public RunnerCollisionService(IRunnerBodyCollision runnerCollision, IReadOnlyGameStateModel gameStateModel)
        {
            _runnerBodyCollision = runnerCollision;
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
                    _runnerBodyCollision.EnableCollisions();
                    break;

                case GameState.Tutorial:
                case GameState.Win:
                case GameState.Lose:
                    _runnerBodyCollision.DisableCollisions();
                    break;

                default:
                    throw new UnhandledRunnerCollisionStateException(state);
            }
        }
    }
}

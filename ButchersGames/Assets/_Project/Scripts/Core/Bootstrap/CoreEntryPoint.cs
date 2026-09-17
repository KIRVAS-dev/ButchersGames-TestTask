using System;
using Core.Input.RunnerMovement;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly CoreCancellationSource _coreCancellation;
        private readonly RunnerMovementInputHandler _runnerMovementInputHandler;

        public CoreEntryPoint(
            IGameplayInputBlock gameplayInputBlock,
            CoreCancellationSource coreCancellation,
            RunnerMovementInputHandler runnerMovementInputHandler)
        {
            _gameplayInputBlock = gameplayInputBlock;
            _coreCancellation = coreCancellation;
            _runnerMovementInputHandler = runnerMovementInputHandler;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();
        }
    }
}

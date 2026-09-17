using System;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly CoreCancellationSource _coreCancellation;

        public CoreEntryPoint(IGameplayInputBlock gameplayInputBlock, CoreCancellationSource coreCancellation)
        {
            _gameplayInputBlock = gameplayInputBlock;
            _coreCancellation = coreCancellation;
        }

        void IStartable.Start() { }

        void IDisposable.Dispose() { }
    }
}

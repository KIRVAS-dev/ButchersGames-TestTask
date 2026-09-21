using System;
using System.Threading;

namespace Core.Lifecycle
{
    public sealed class CoreScopeCancellationSource
        : ICoreScopeCancellation,
          IDisposable
    {
        public CancellationToken Token => _cancellationTokenSource.Token;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        void IDisposable.Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}

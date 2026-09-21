using System.Threading;

namespace Core.Lifecycle
{
    public interface ICoreScopeCancellation
    {
        CancellationToken Token { get; }
    }
}

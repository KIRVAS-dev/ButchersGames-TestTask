using System.Threading;

namespace Core
{
    public interface ICoreScopeCancellation
    {
        CancellationToken Token { get; }
    }
}

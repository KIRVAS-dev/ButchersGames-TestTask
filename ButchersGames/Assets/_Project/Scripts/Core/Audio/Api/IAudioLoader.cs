using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Audio
{
    public interface IAudioLoader
    {
        UniTask LoadAsync(CancellationToken cancellationToken);
    }
}

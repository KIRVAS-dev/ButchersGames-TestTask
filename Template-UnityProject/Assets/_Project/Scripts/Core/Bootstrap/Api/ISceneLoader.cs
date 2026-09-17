using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Bootstrap
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, CancellationToken cancellationToken);
    }
}

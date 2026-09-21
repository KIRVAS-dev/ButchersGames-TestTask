using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Bootstrap.Scene
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, CancellationToken cancellationToken);
    }
}

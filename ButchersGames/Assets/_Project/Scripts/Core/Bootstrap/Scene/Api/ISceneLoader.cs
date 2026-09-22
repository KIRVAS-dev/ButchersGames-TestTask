using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Bootstrap.Scene
{
    public interface ISceneLoader
    {
        void LoadScene(string sceneName, LoadSceneMode loadSceneMode);

        UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken);
    }
}

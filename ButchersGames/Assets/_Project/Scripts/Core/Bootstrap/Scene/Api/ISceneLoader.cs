using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Bootstrap.Scene
{
    public interface ISceneLoader
    {
        void Load(string sceneName, LoadSceneMode loadSceneMode);

        UniTask LoadAsync(
            string sceneName,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken);

        void SetActiveScene(string sceneName);
    }
}

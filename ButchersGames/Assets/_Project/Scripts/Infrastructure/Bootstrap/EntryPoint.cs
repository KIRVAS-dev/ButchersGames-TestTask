using Core.Bootstrap.Scene;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint : IStartable
    {
        private const string CoreSceneName = "Core";

        private readonly ISceneLoader _sceneLoader;

        public EntryPoint(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Start()
        {
            LoadCoreAsync().Forget();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            await _sceneLoader.LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive, CancellationToken.None);
        }
    }
}

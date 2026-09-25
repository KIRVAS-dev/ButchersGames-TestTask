using Core.Audio;
using Core.Bootstrap.Scene;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint : IStartable
    {
        private const string CoreSceneName = "Core";

        private readonly IAudioLoader _audioLoader;
        private readonly ISceneLoader _sceneLoader;

        public EntryPoint(IAudioLoader audioLoader, ISceneLoader sceneLoader)
        {
            _audioLoader = audioLoader;
            _sceneLoader = sceneLoader;
        }

        void IStartable.Start()
        {
            LoadCoreAsync().Forget();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            await _audioLoader.LoadAsync(CancellationToken.None);
            await _sceneLoader.LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive, CancellationToken.None);
        }
    }
}

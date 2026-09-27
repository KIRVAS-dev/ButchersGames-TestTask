using System;
using System.Threading;
using Core.Bootstrap;
using Core.Bootstrap.Scene;
using Cysharp.Threading.Tasks;
using Infrastructure.Audio;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint
        : IStartable,
          IDisposable
    {
        private const string CoreSceneName = "Core";

        private readonly IAudioLoader _audioLoader;
        private readonly ISceneLoader _sceneLoader;
        private readonly ScopeLifecycle _scopeLifecycle;

        public EntryPoint(
            IAudioLoader audioLoader,
            ISceneLoader sceneLoader,
            ScopeLifecycle scopeLifecycle)
        {
            _audioLoader = audioLoader;
            _sceneLoader = sceneLoader;
            _scopeLifecycle = scopeLifecycle;
        }

        void IStartable.Start()
        {
            _scopeLifecycle.Start();

            LoadCoreAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _scopeLifecycle.Stop();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            await _audioLoader.LoadAsync(CancellationToken.None);
            await _sceneLoader.LoadAsync(CoreSceneName, LoadSceneMode.Additive, CancellationToken.None);

            _sceneLoader.SetActiveScene(CoreSceneName);
        }
    }
}

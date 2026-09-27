using Core.Bootstrap.Scene;
using Core.Validation;
using Cysharp.Threading.Tasks;
using Infrastructure.Audio;
using System.Threading;
using UI.LoadingScreen;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint : IStartable
    {
        private const string CoreSceneName = "Core";

        private readonly IAudioLoader _audioLoader;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingScreenView _loadingScreenView;
        private readonly SessionValidation _sessionValidation;

        public EntryPoint(
            IAudioLoader audioLoader,
            ISceneLoader sceneLoader,
            ILoadingScreenView loadingScreenView,
            SessionValidation sessionValidation)
        {
            _audioLoader = audioLoader;
            _sceneLoader = sceneLoader;
            _loadingScreenView = loadingScreenView;
            _sessionValidation = sessionValidation;
        }

        void IStartable.Start()
        {
            _sessionValidation.Validate();
            _loadingScreenView.Show();

            LoadCoreAsync().Forget();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            await _audioLoader.LoadAsync(CancellationToken.None);
            await _sceneLoader.LoadAsync(CoreSceneName, LoadSceneMode.Additive, CancellationToken.None);

            _sceneLoader.SetActiveScene(CoreSceneName);
        }
    }
}

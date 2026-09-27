using ContentValidation;
using Core.Bootstrap.Scene;
using Core.Validation;
using Infrastructure.Audio;
using Infrastructure.ExtendedExceptions;
using UI.LoadingScreen;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;

namespace Infrastructure.Bootstrap
{
    internal sealed class ProjectScope : LifetimeScope
    {
        [SerializeField] private Transform _studioListener;
        [SerializeField] private LoadingScreenView _loadingScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterSceneLoading(builder);
            RegisterAudio(builder);
            RegisterLoadingScreen(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
            builder.Register<SessionValidation>(Lifetime.Singleton);
        }

        private static void RegisterSceneLoading(IContainerBuilder builder)
        {
            builder.Register<CoreLoader>(Lifetime.Singleton).As<ISceneLoader>();
        }

        private void RegisterAudio(IContainerBuilder builder)
        {
            Guard.AgainstNull(_studioListener, () => new MissingStudioListenerAnchorTransformException());

            builder.RegisterInstance(new StudioListenerAnchor(_studioListener)).As<IStudioListenerAnchor>();
            builder.Register<FmodAudioLoader>(Lifetime.Singleton).As<IAudioLoader>();
        }

        private void RegisterLoadingScreen(IContainerBuilder builder)
        {
            Guard.AgainstNull(_loadingScreenView, () => new MissingLoadingScreenViewException());

            builder.RegisterComponent(_loadingScreenView).As<ILoadingScreenView>().As<IValidatable>();
        }
    }
}

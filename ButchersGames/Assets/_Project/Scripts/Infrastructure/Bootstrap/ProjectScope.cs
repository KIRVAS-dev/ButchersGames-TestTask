using Core.Audio;
using Core.Bootstrap.Scene;
using Infrastructure.Audio;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class ProjectScope : LifetimeScope
    {
        [SerializeField] private Transform _studioListener;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterSceneLoading(builder);
            RegisterAudio(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
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
    }
}

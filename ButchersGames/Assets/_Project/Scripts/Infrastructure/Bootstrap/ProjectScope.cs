using Core.Audio;
using Core.Bootstrap.Scene;
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
            Guard.AgainstNull(_studioListener, () => new MissingStudioListenerAnchorTransformException());

            builder.RegisterEntryPoint<EntryPoint>();
            builder.Register<CoreLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.RegisterInstance(new StudioListenerAnchor(_studioListener)).As<IStudioListenerAnchor>();
        }
    }
}

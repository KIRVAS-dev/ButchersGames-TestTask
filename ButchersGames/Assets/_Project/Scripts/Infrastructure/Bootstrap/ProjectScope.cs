using Core.Bootstrap;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    public sealed class ProjectScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
            builder.Register<CoreLoader>(Lifetime.Singleton).As<ISceneLoader>();
        }
    }
}

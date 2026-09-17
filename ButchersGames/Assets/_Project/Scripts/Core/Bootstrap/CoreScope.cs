using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
        }
    }
}

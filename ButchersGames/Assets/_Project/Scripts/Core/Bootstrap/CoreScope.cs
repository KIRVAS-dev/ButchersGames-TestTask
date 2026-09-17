using Core.Gameplay.LevelProgression;
using ViewComponents.Level;
using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
            builder.Register<CoreCancellationSource>(Lifetime.Singleton);

            builder.RegisterEntryPoint<CoreEntryPoint>();

            RegisterLevel(builder);
        }

        private void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LevelProvider>().As<ILevelProvider>();
            builder.RegisterComponentInHierarchy<LevelView>().As<ILevelView>();
            builder.Register<PlayerPrefsLevelProgressStore>(Lifetime.Singleton).As<ILevelProgressStore>();
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>();
        }
    }
}

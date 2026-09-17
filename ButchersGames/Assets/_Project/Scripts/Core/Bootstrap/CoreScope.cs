using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using ExtendedExceptions;
using ViewComponents.Level;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private WealthMeterConfig _wealthMeterConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterLevelProgression(builder);
            RegisterWealthMeter(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
            builder.Register<CoreCancellationSource>(Lifetime.Singleton);
        }

        private void RegisterLevelProgression(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LevelProvider>().As<ILevelProvider>();
            builder.RegisterComponentInHierarchy<LevelView>().As<ILevelView>();
            builder.Register<PlayerPrefsLevelProgressStore>(Lifetime.Singleton).As<ILevelProgressStore>();
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>();
        }

        private void RegisterWealthMeter(IContainerBuilder builder)
        {
            Guard.AgainstNull(
                _wealthMeterConfig,
                () => new MissingWealthMeterConfigException(nameof(_wealthMeterConfig), gameObject.name)
            );

            _wealthMeterConfig.Validate();

            builder.RegisterInstance(_wealthMeterConfig);
            builder.Register<WealthMeterModel>(Lifetime.Singleton);
            builder.Register<WealthMeterService>(Lifetime.Singleton).As<IWealthMeterService>();
        }
    }
}

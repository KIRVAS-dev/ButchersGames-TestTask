using Core.Gameplay.Finish;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthMeter;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using ExtendedExceptions;
using Infrastructure.Persistence;
using Input;
using ViewComponents.Finish;
using ViewComponents.LaneBarriers;
using ViewComponents.Level;
using ViewComponents.Obstacles;
using ViewComponents.RunnerMovement;
using ViewComponents.WealthMeter;
using ViewComponents.WealthPointsModifier;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Core.Bootstrap
{
    public sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private WealthMeterConfig _wealthMeterConfig;
        [SerializeField] private RunnerMovementConfig _runnerMovementConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterLevelProgression(builder);
            RegisterWealthMeter(builder);
            RegisterWealthPointsModifier(builder);
            RegisterObstacle(builder);
            RegisterLaneBarrier(builder);
            RegisterFinish(builder);
            RegisterRunnerMovement(builder);
            RegisterGameFlow(builder);
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

            builder.RegisterInstance<IWealthMeterSettings>(_wealthMeterConfig);
            builder.Register<WealthMeterModel>(Lifetime.Singleton);
            builder.Register<WealthMeterService>(Lifetime.Singleton).As<IWealthMeterService>();
        }

        private static void RegisterWealthPointsModifier(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<WealthPointsModifierRegistry>().As<IWealthPointsModifierRegistry>();
            builder.Register<WealthPointsModifierService>(Lifetime.Singleton).AsSelf();
        }

        private static void RegisterObstacle(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ObstacleRegistry>().As<IObstacleRegistry>();
        }

        private static void RegisterLaneBarrier(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LaneBarrierRegistry>().As<ILaneBarrierRegistry>();
        }

        private static void RegisterFinish(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<FinishProvider>().As<IFinishProvider>();
        }

        private void RegisterRunnerMovement(IContainerBuilder builder)
        {
            Guard.AgainstNull(
                _runnerMovementConfig,
                () => new MissingRunnerMovementConfigException(nameof(_runnerMovementConfig), gameObject.name)
            );

            _runnerMovementConfig.Validate();

            builder.RegisterInstance<IRunnerMovementSettings>(_runnerMovementConfig);
            builder.RegisterComponentInHierarchy<DragInput>().As<IDragInput>();
            builder.RegisterComponentInHierarchy<RunnerMovementView>().AsSelf();
            builder.RegisterComponentInHierarchy<RunnerTrackFollower>().AsSelf();
            builder.Register<RunnerMovementModel>(Lifetime.Singleton);
            builder.Register<RunnerMovementService>(Lifetime.Singleton).As<IRunnerMovementService>();
            builder.Register<RunnerMovementInputHandler>(Lifetime.Singleton);
        }

        private static void RegisterGameFlow(IContainerBuilder builder)
        {
            builder.Register<GameFlowModel>(Lifetime.Singleton);
            builder.Register<GameFlowService>(Lifetime.Singleton).As<IGameFlowService>().AsSelf();
        }
    }
}

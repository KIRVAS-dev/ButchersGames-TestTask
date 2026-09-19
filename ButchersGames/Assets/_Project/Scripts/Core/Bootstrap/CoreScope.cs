using Core.Gameplay.Feedback;
using Core.Gameplay.Finish;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthMeter;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using Infrastructure.ExtendedExceptions;
using Infrastructure.Persistence;
using Input;
using UI.FloatingText;
using UI.Hud;
using UI.ResultScreen;
using UI.StartScreen;
using ViewComponents.Feedback;
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
            RegisterLevel(builder);
            RegisterWealthMeter(builder);
            RegisterRunnerMovement(builder);
            RegisterGameFlow(builder);
            RegisterFeedback(builder);
            RegisterUi(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
        }

        private static void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LevelProvider>().As<ILevelProvider>().AsSelf();
            builder.RegisterComponentInHierarchy<LevelView>().As<ILevelView>();
            builder.Register<PlayerPrefsLevelProgressStore>(Lifetime.Singleton).As<ILevelProgressStore>();
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>();
            builder.Register<WealthPointsModifierRegistry>(Lifetime.Singleton).As<IWealthPointsModifierRegistry>();
            builder.Register<WealthPointsModifierService>(Lifetime.Singleton).AsSelf();
            builder.Register<ObstacleRegistry>(Lifetime.Singleton).As<IObstacleRegistry>();
            builder.Register<LaneBarrierRegistry>(Lifetime.Singleton).As<ILaneBarrierRegistry>();
            builder.Register<FinishProvider>(Lifetime.Singleton).As<IFinishProvider>();
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
            builder.RegisterComponentInHierarchy<CharacterAppearanceView>().As<ICharacterAppearanceView>();
            builder.Register<CharacterAppearancePresenter>(Lifetime.Singleton);
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
            builder.RegisterComponentOnNewGameObject<TickInput>(Lifetime.Singleton, nameof(TickInput)).As<ITickInput>();
            builder.RegisterComponentInHierarchy<RunnerMovementView>().AsSelf();
            builder.RegisterComponentInHierarchy<RunnerTrackFollowerView>().As<IRunnerTrackFollowerView>();
            builder.Register<RunnerMovementModel>(Lifetime.Singleton);
            builder.Register<RunnerMovementService>(Lifetime.Singleton).As<IRunnerMovementService>().AsSelf();
            builder.Register<RunnerMovementInputHandler>(Lifetime.Singleton);
            builder.Register<RunnerMovementPresenter>(Lifetime.Singleton);
        }

        private static void RegisterGameFlow(IContainerBuilder builder)
        {
            builder.Register<GameFlowModel>(Lifetime.Singleton);
            builder.Register<GameFlowService>(Lifetime.Singleton).As<IGameFlowService>().AsSelf();
        }

        private static void RegisterFeedback(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<FeedbackPerformer>().As<IFeedbackPerformer>();
            builder.Register<FeedbackPresenter>(Lifetime.Singleton);
        }

        private static void RegisterUi(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<StartScreenView>().As<IStartScreenView>();
            builder.Register<StartScreenPresenter>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<HudView>().As<IHudView>();
            builder.Register<HudPresenter>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<ResultScreenView>().As<IResultScreenView>();
            builder.Register<ResultScreenPresenter>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<FloatingTextView>().As<IFloatingTextView>();
            builder.Register<FloatingTextPresenter>(Lifetime.Singleton);
        }
    }
}

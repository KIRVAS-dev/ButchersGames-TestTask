using ContentValidation;
using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.Track;
using Core.Gameplay.TransformRotator;
using Core.Gameplay.WealthMeter;
using Core.Gameplay.WealthPointsModifier;
using Core.Input;
using Core.Input.RunnerMovement;
using Core.Lifecycle;
using Core.Loop;
using Core.Validation;
using Infrastructure.ExtendedExceptions;
using Infrastructure.Persistence;
using Input;
using UI.FloatingText;
using UI.Hud;
using UI.ResultScreen;
using UI.StartScreen;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;
using ViewComponents.CelebrationCamera;
using ViewComponents.CharacterAnimation;
using ViewComponents.CharacterTurn;
using ViewComponents.Feedback;
using ViewComponents.Level;
using ViewComponents.RunnerMovement;
using ViewComponents.TransformRotators;
using ViewComponents.WealthMeter;

namespace Core.Bootstrap
{
    internal sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private WealthMeterConfig _wealthMeterConfig;
        [SerializeField] private RunnerMovementConfig _runnerMovementConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterGameFlow(builder);
            RegisterWealthMeter(builder);
            RegisterRunnerMovement(builder);
            RegisterLevel(builder);
            RegisterWealthPointsModifier(builder);
            RegisterTransformRotators(builder);
            RegisterCharacterAnimation(builder);
            RegisterCharacterTurn(builder);
            RegisterCelebrationCamera(builder);
            RegisterStudioListenerFollow(builder);
            RegisterFeedbackPresentation(builder);
            RegisterUi(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
            builder.RegisterEntryPoint<GameLoop>();
            builder.Register<SessionValidation>(Lifetime.Singleton);
        }

        private static void RegisterGameFlow(IContainerBuilder builder)
        {
            builder.Register<GameStateModel>(Lifetime.Singleton);
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
            builder.Register<GameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>();
            builder.Register<GameResultDetector>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
            builder.Register<GameFlowService>(Lifetime.Singleton).As<IGameFlowService>();
        }

        private void RegisterWealthMeter(IContainerBuilder builder)
        {
            Guard.AgainstNull(
                _wealthMeterConfig,
                () => new MissingWealthMeterConfigException(nameof(_wealthMeterConfig), gameObject.name)
            );

            builder.RegisterInstance(_wealthMeterConfig).As<IWealthMeterSettings>().As<IValidatable>();
            builder.Register<WealthMeterModel>(Lifetime.Singleton);
            builder.Register<WealthMeterService>(Lifetime.Singleton).As<IWealthMeterService>().As<ISubscriptionLifecycle>();
            builder.RegisterComponentInHierarchy<CharacterAppearanceView>().As<ICharacterAppearanceView>().As<IValidatable>();
            builder.Register<CharacterAppearancePresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private void RegisterRunnerMovement(IContainerBuilder builder)
        {
            Guard.AgainstNull(
                _runnerMovementConfig,
                () => new MissingRunnerMovementConfigException(nameof(_runnerMovementConfig), gameObject.name)
            );

            builder
               .RegisterInstance(_runnerMovementConfig)
               .As<IRunnerMovementSettings>()
               .As<IRunnerMovementInputSettings>()
               .As<IValidatable>();

            builder.Register<DragInput>(Lifetime.Singleton).As<IDragInput>().As<IInputTickable>();
            builder.RegisterComponentInHierarchy<RunnerMovementView>().As<IRunnerMovementView>().As<IPresentationTickable>();
            builder.Register<RunnerMovementModel>(Lifetime.Singleton);

            builder
               .Register<RunnerMovementService>(Lifetime.Singleton)
               .As<IRunnerMovementService>()
               .As<IGameplayTickable>()
               .As<ISubscriptionLifecycle>();

            builder.Register<RunnerMovementInputHandler>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
            builder.Register<RunnerMovementPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LevelLoader>().As<ILevelLoader>().AsSelf().As<IValidatable>();

            builder
               .Register<CurrentLevel>(Lifetime.Singleton)
               .As<IObstacleRegistry>()
               .As<ILaneBarrierRegistry>()
               .As<IWealthPointsModifierRegistry>()
               .As<ITransformRotatorsRegistry>()
               .As<ITrackProvider>()
               .AsSelf();

            builder.Register<PlayerPrefsLevelProgressStore>(Lifetime.Singleton).As<ILevelProgressStore>();
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>();
        }

        private static void RegisterWealthPointsModifier(IContainerBuilder builder)
        {
            builder.Register<WealthPointsModifierService>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterTransformRotators(IContainerBuilder builder)
        {
            builder.Register<TransformRotatorsTicker>(Lifetime.Singleton).As<IPresentationTickable>();
        }

        private static void RegisterCharacterAnimation(IContainerBuilder builder)
        {
            builder
               .RegisterComponentInHierarchy<CharacterAnimationView>()
               .As<ICharacterAnimationView>()
               .As<IValidatable>()
               .As<IWarmupLifecycle>();

            builder.Register<CharacterAnimationPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterCharacterTurn(IContainerBuilder builder)
        {
            builder
               .RegisterComponentInHierarchy<CharacterTurnView>()
               .As<ICharacterTurnView>()
               .As<IPresentationTickable>()
               .As<IValidatable>()
               .As<IWarmupLifecycle>();

            builder.Register<CharacterTurnPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterCelebrationCamera(IContainerBuilder builder)
        {
            builder
               .RegisterComponentInHierarchy<CelebrationCameraView>()
               .As<ICelebrationCameraView>()
               .As<IValidatable>()
               .As<IWarmupLifecycle>();

            builder.Register<CelebrationCameraPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterStudioListenerFollow(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<StudioListenerCameraFollower>();
        }

        private static void RegisterFeedbackPresentation(IContainerBuilder builder)
        {
            builder
               .RegisterComponentInHierarchy<FeedbackPerformer>()
               .As<IFeedbackPerformer>()
               .As<IValidatable>()
               .As<IWarmupLifecycle>();

            builder.Register<FeedbackPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private static void RegisterUi(IContainerBuilder builder)
        {
            builder
               .RegisterComponentInHierarchy<StartScreenView>()
               .As<IStartScreenView>()
               .As<IValidatable>()
               .As<ISubscriptionLifecycle>();

            builder.Register<StartScreenPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
            builder.RegisterComponentInHierarchy<HudView>().As<IHudView>().As<IValidatable>();
            builder.Register<HudPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder
               .RegisterComponentInHierarchy<ResultScreenView>()
               .As<IResultScreenView>()
               .As<IValidatable>()
               .As<ISubscriptionLifecycle>();

            builder.Register<ResultScreenPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder
               .RegisterComponentInHierarchy<FloatingTextView>()
               .As<IFloatingTextView>()
               .As<IValidatable>()
               .As<IWarmupLifecycle>();

            builder.Register<FloatingTextPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }
    }
}

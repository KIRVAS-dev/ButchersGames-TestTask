using System;
using System.Collections.Generic;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthPointsModifier;
using Core.Lifecycle;
using R3;

namespace ViewComponents.CharacterAnimation
{
    public sealed class CharacterAnimationPresenter : ISubscriptionLifecycle
    {
        private readonly ICharacterAnimationView _view;
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IReadOnlyRunnerMovementModel _runnerMovementModel;

        private IDisposable _slotSubscription;
        private IReadOnlyCollection<IObstacle> _subscribedObstacles = Array.Empty<IObstacle>();

        public CharacterAnimationPresenter(
            ICharacterAnimationView view,
            ILevelLoaderEvents levelLoaderEvents,
            IObstacleRegistry obstacleRegistry,
            IReadOnlyGameStateModel gameStateModel,
            IReadOnlyRunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _levelLoaderEvents = levelLoaderEvents;
            _obstacleRegistry = obstacleRegistry;
            _gameStateModel = gameStateModel;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;

            _slotSubscription = Observable
               .CombineLatest(_gameStateModel.State, _runnerMovementModel.State, PlaySlotFor)
               .DistinctUntilChanged()
               .Subscribe(_view.Play);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;

            UnsubscribeObstacles();
            _slotSubscription?.Dispose();
        }

        private static CharacterAnimationSlot ReactionSlotFor(WealthPointsModifierType modifierType)
        {
            switch (modifierType)
            {
                case WealthPointsModifierType.Increase:
                    return CharacterAnimationSlot.Happy;

                case WealthPointsModifierType.Decrease:
                    return CharacterAnimationSlot.Sad;

                default:
                    throw new UnhandledWealthPointsModifierTypeException(modifierType);
            }
        }

        private static CharacterAnimationSlot PlaySlotFor(GameState gameState, RunnerMovementState movementState)
        {
            switch (gameState)
            {
                case GameState.Tutorial:
                    return CharacterAnimationSlot.Idle;

                case GameState.Run:
                    return movementState == RunnerMovementState.Moving
                        ? CharacterAnimationSlot.Run
                        : CharacterAnimationSlot.GetHit;

                case GameState.Win:
                    return CharacterAnimationSlot.Victory;

                case GameState.Lose:
                    return CharacterAnimationSlot.Defeat;

                default:
                    throw new UnhandledCharacterAnimationStateException(gameState);
            }
        }

        private void OnLevelLoaded()
        {
            UnsubscribeObstacles();

            _subscribedObstacles = _obstacleRegistry.Obstacles;

            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Modifier.Triggered += OnObstacleModifierTriggered;
            }
        }

        private void UnsubscribeObstacles()
        {
            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Modifier.Triggered -= OnObstacleModifierTriggered;
            }

            _subscribedObstacles = Array.Empty<IObstacle>();
        }

        private void OnObstacleModifierTriggered(WealthPointsModifierType modifierType, int wealthPoints)
        {
            _view.SetReaction(ReactionSlotFor(modifierType));
        }
    }
}

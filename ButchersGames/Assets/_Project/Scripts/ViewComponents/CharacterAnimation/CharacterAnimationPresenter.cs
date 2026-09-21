using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthPointsModifier;
using R3;

namespace ViewComponents.CharacterAnimation
{
    public sealed class CharacterAnimationPresenter
    {
        private readonly ICharacterAnimationView _view;
        private readonly ILevelLoader _levelLoader;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly GameStateModel _gameStateModel;
        private readonly RunnerMovementModel _runnerMovementModel;

        private IDisposable _slotSubscription;

        public CharacterAnimationPresenter(
            ICharacterAnimationView view,
            ILevelLoader levelLoader,
            IObstacleRegistry obstacleRegistry,
            GameStateModel gameStateModel,
            RunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _levelLoader = levelLoader;
            _obstacleRegistry = obstacleRegistry;
            _gameStateModel = gameStateModel;
            _runnerMovementModel = runnerMovementModel;
        }

        public void StartListening()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;

            _slotSubscription = Observable
               .CombineLatest(_gameStateModel.State, _runnerMovementModel.State, PlaySlotFor)
               .DistinctUntilChanged()
               .Subscribe(_view.Play);
        }

        public void StopListening()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;

            UnsubscribeObstacles();
            _slotSubscription?.Dispose();
        }

        private void OnLevelLoaded()
        {
            foreach (IObstacle obstacle in _obstacleRegistry.Obstacles)
            {
                obstacle.Modifier.Triggered += OnObstacleModifierTriggered;
            }
        }

        private void UnsubscribeObstacles()
        {
            if (_obstacleRegistry.Obstacles == null)
            {
                return;
            }

            foreach (IObstacle obstacle in _obstacleRegistry.Obstacles)
            {
                obstacle.Modifier.Triggered -= OnObstacleModifierTriggered;
            }
        }

        private void OnObstacleModifierTriggered(WealthPointsModifierType modifierType, int wealthPoints)
        {
            _view.SetReaction(ReactionSlotFor(modifierType));
        }

        private CharacterAnimationSlot ReactionSlotFor(WealthPointsModifierType modifierType)
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

        private CharacterAnimationSlot PlaySlotFor(GameState gameState, RunnerMovementState movementState)
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
    }
}

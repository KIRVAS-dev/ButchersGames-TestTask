using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using R3;

namespace ViewComponents.CharacterAnimation
{
    public sealed class CharacterAnimationPresenter
    {
        private readonly ICharacterAnimationView _view;
        private readonly GameStateModel _gameStateModel;
        private readonly RunnerMovementModel _runnerMovementModel;

        private IDisposable _slotSubscription;

        public CharacterAnimationPresenter(
            ICharacterAnimationView view,
            GameStateModel gameStateModel,
            RunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _gameStateModel = gameStateModel;
            _runnerMovementModel = runnerMovementModel;
        }

        public void StartListening()
        {
            _slotSubscription = Observable
               .CombineLatest(_gameStateModel.State, _runnerMovementModel.State, SlotFor)
               .DistinctUntilChanged()
               .Subscribe(_view.Play);
        }

        public void StopListening()
        {
            _slotSubscription?.Dispose();
        }

        private CharacterAnimationSlot SlotFor(GameState gameState, RunnerMovementState movementState)
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

using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace ViewComponents.CharacterAnimation
{
    public sealed class CharacterAnimationPresenter : ISubscriptionLifecycle
    {
        private readonly ICharacterAnimationView _view;
        private readonly IWealthMeterEvents _wealthMeterEvents;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly IReadOnlyRunnerMovementModel _runnerMovementModel;
        private readonly ReactiveProperty<CharacterAnimationSlot> _stopReaction =
            new ReactiveProperty<CharacterAnimationSlot>(CharacterAnimationSlot.Sad);

        private IDisposable _slotSubscription;

        public CharacterAnimationPresenter(
            ICharacterAnimationView view,
            IWealthMeterEvents wealthMeterEvents,
            IReadOnlyGameStateModel gameStateModel,
            IReadOnlyRunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _wealthMeterEvents = wealthMeterEvents;
            _gameStateModel = gameStateModel;
            _runnerMovementModel = runnerMovementModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Increased += OnWealthIncreased;
            _wealthMeterEvents.Decreased += OnWealthDecreased;

            _slotSubscription = Observable
               .CombineLatest(_gameStateModel.State, _runnerMovementModel.State, _stopReaction, PlaySlotFor)
               .DistinctUntilChanged()
               .Subscribe(_view.Play);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Increased -= OnWealthIncreased;
            _wealthMeterEvents.Decreased -= OnWealthDecreased;

            _slotSubscription?.Dispose();
            _stopReaction.Dispose();
        }

        private static CharacterAnimationSlot PlaySlotFor(
            GameState gameState,
            RunnerMovementState movementState,
            CharacterAnimationSlot stopReaction)
        {
            switch (gameState)
            {
                case GameState.Tutorial:
                    return CharacterAnimationSlot.Idle;

                case GameState.Run:
                    return movementState == RunnerMovementState.Moving
                        ? CharacterAnimationSlot.Run
                        : stopReaction;

                case GameState.Win:
                    return CharacterAnimationSlot.Victory;

                case GameState.Lose:
                    return CharacterAnimationSlot.Defeat;

                default:
                    throw new UnhandledCharacterAnimationStateException(gameState);
            }
        }

        private void OnWealthIncreased(int amount)
        {
            _stopReaction.Value = CharacterAnimationSlot.Happy;
        }

        private void OnWealthDecreased(int amount)
        {
            _stopReaction.Value = CharacterAnimationSlot.Sad;
        }
    }
}

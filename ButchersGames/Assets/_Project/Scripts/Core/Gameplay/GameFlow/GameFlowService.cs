using System;
using Core.Gameplay.Finish;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameFlowService : IGameFlowService
    {
        private readonly ILevelService _levelService;
        private readonly IWealthMeterService _wealthMeter;
        private readonly IFinishProvider _finishProvider;
        private readonly IGameplayInputBlock _inputBlock;
        private readonly GameFlowModel _model;

        public GameFlowService(
            ILevelService levelService,
            IWealthMeterService wealthMeter,
            IFinishProvider finishProvider,
            IGameplayInputBlock inputBlock,
            GameFlowModel model)
        {
            _levelService = levelService;
            _wealthMeter = wealthMeter;
            _finishProvider = finishProvider;
            _inputBlock = inputBlock;
            _model = model;
        }

        public GameFlowState State => _model.State.Value;

        public void StartListening()
        {
            SubscribeToEvents();
            EnterWaitingToStart(_levelService.SelectCurrentLevel);
        }

        public void StopListening()
        {
            UnsubscribeFromEvents();
        }

        public void StartGame()
        {
            Guard.AgainstTrue(
                State != GameFlowState.WaitingToStart,
                () => new InvalidGameFlowTransitionException(nameof(StartGame), State)
            );

            _model.State.Value = GameFlowState.Playing;
            _inputBlock.Unblock();
        }

        public void RetryLevel()
        {
            Guard.AgainstTrue(
                State != GameFlowState.Lose,
                () => new InvalidGameFlowTransitionException(nameof(RetryLevel), State)
            );

            EnterWaitingToStart(_levelService.RestartLevel);
        }

        public void ProceedToNextLevel()
        {
            Guard.AgainstTrue(
                State != GameFlowState.Win,
                () => new InvalidGameFlowTransitionException(nameof(ProceedToNextLevel), State)
            );

            EnterWaitingToStart(_levelService.ProceedToNextLevel);
        }

        private void EnterWaitingToStart(Action loadLevel)
        {
            _model.State.Value = GameFlowState.WaitingToStart;
            _inputBlock.Block();

            loadLevel();
        }

        private void SubscribeToEvents()
        {
            _wealthMeter.Depleted += OnWealthDepleted;
            _finishProvider.Reached += OnFinishReached;
        }

        private void UnsubscribeFromEvents()
        {
            _wealthMeter.Depleted -= OnWealthDepleted;
            _finishProvider.Reached -= OnFinishReached;
        }

        private void OnWealthDepleted()
        {
            _model.State.Value = GameFlowState.Lose;
            _inputBlock.Block();
        }

        private void OnFinishReached()
        {
            _model.State.Value = GameFlowState.Win;
            _inputBlock.Block();
        }
    }
}

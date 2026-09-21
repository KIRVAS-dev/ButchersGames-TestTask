using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using R3;

namespace ViewComponents.CharacterTurn
{
    public sealed class CharacterTurnPresenter
    {
        private readonly ICharacterTurnView _view;
        private readonly GameStateModel _gameStateModel;
        private readonly RunnerMovementModel _runnerMovementModel;

        private IDisposable _sideSubscription;

        public CharacterTurnPresenter(
            ICharacterTurnView view,
            GameStateModel gameStateModel,
            RunnerMovementModel runnerMovementModel)
        {
            _view = view;
            _gameStateModel = gameStateModel;
            _runnerMovementModel = runnerMovementModel;
        }

        public void StartListening()
        {
            _sideSubscription = Observable
               .CombineLatest(
                    _gameStateModel.State,
                    _runnerMovementModel.State,
                    _runnerMovementModel.LateralDirection,
                    SideFor)
               .DistinctUntilChanged()
               .Subscribe(_view.SetTurn);
        }

        public void StopListening()
        {
            _sideSubscription?.Dispose();
        }

        private CharacterTurnSide SideFor(
            GameState gameState,
            RunnerMovementState movementState,
            RunnerLateralDirection direction)
        {
            if (gameState != GameState.Run || movementState != RunnerMovementState.Moving)
            {
                return CharacterTurnSide.None;
            }

            switch (direction)
            {
                case RunnerLateralDirection.None:
                    return CharacterTurnSide.None;

                case RunnerLateralDirection.Left:
                    return CharacterTurnSide.Left;

                case RunnerLateralDirection.Right:
                    return CharacterTurnSide.Right;

                default:
                    throw new UnhandledCharacterTurnDirectionException(direction);
            }
        }
    }
}

using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly GameStateModel _model;

        public GameStateMachine(GameStateModel model)
        {
            _model = model;
        }

        public GameState State => _model.State.Value;

        public void EnterState(GameState state)
        {
            Guard.AgainstTrue(
                !IsTransitionAllowed(state),
                () => new InvalidGameStateTransitionException(state.ToString(), State)
            );

            _model.State.Value = state;
        }

        private bool IsTransitionAllowed(GameState state)
        {
            return state switch
            {
                GameState.Tutorial => State != GameState.Run,
                GameState.Run => State == GameState.Tutorial,
                GameState.Win or GameState.Lose => State == GameState.Run,
                _ => throw new UnhandledGameStateException(state)
            };
        }
    }
}

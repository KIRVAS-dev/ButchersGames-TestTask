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

        GameState IGameStateMachine.State => _model.State.Value;

        void IGameStateMachine.EnterState(GameState state)
        {
            Guard.AgainstTrue(
                !IsTransitionAllowed(state),
                () => new InvalidGameStateTransitionException(state.ToString(), _model.State.Value)
            );

            _model.State.Value = state;
        }

        private bool IsTransitionAllowed(GameState state)
        {
            return state switch
            {
                GameState.Tutorial => _model.State.Value != GameState.Run,
                GameState.Run => _model.State.Value == GameState.Tutorial,
                GameState.Win or GameState.Lose => _model.State.Value == GameState.Run,
                _ => throw new UnhandledGameStateException(state)
            };
        }
    }
}

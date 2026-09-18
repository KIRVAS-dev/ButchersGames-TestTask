using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.GameFlow
{
    public sealed class InvalidGameFlowTransitionException : ExtendedException
    {
        public InvalidGameFlowTransitionException(string attemptedTransition, GameFlowState currentState)
            : base("game-flow-1", $"Transition '{attemptedTransition}' is not valid from state '{currentState}'") { }
    }
}

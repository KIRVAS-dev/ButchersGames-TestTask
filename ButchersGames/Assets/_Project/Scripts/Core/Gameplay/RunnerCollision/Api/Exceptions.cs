using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.RunnerCollision
{
    internal sealed class UnhandledRunnerCollisionStateException : ExtendedException
    {
        internal UnhandledRunnerCollisionStateException(GameState state)
            : base("runner-collision-service-1", $"GameState '{state}' is not handled by the runner collision service") { }
    }
}

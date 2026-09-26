using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.RunnerCollision
{
    internal sealed class MissingRunnerCollisionFieldException : ExtendedException
    {
        internal MissingRunnerCollisionFieldException(string fieldName, string objectName)
            : base("runner-collision-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class UnhandledRunnerCollisionStateException : ExtendedException
    {
        internal UnhandledRunnerCollisionStateException(GameState state)
            : base("runner-collision-2", $"GameState '{state}' is not handled by the runner collision presenter") { }
    }
}

using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class InvalidRunnerMovementValueException : ExtendedException
    {
        public InvalidRunnerMovementValueException(string fieldName, float value)
            : base("runner-movement-1", $"RunnerMovementConfig field '{fieldName}' has invalid value {value}") { }
    }

    public sealed class MissingRunnerMovementConfigException : ExtendedException
    {
        public MissingRunnerMovementConfigException(string fieldName, string objectName)
            : base("runner-movement-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

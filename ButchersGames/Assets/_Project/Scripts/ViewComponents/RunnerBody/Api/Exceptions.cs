using Infrastructure.ExtendedExceptions;

namespace ViewComponents.RunnerBody
{
    internal sealed class MissingRunnerBodyFieldException : ExtendedException
    {
        public MissingRunnerBodyFieldException(string fieldName, string objectName)
            : base("runner-body-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidRunnerBodyValueException : ExtendedException
    {
        public InvalidRunnerBodyValueException(string fieldName, string objectName, float value)
            : base("runner-body-2", $"Field '{fieldName}' on '{objectName}' has invalid value {value}") { }
    }
}

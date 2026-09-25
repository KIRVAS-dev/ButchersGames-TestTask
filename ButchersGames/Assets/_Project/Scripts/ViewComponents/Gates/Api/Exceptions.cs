using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Gates
{
    internal sealed class MissingGateFieldException : ExtendedException
    {
        public MissingGateFieldException(string fieldName, string objectName)
            : base("gate-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidGateValueException : ExtendedException
    {
        public InvalidGateValueException(
            string fieldName,
            string objectName,
            int value)
            : base("gate-2", $"Field '{fieldName}' has invalid value '{value}' on '{objectName}'") { }
    }

    internal sealed class InvalidGateSideColliderException : ExtendedException
    {
        public InvalidGateSideColliderException(string objectName)
            : base("gate-3", $"Collider on '{objectName}' must have isTrigger enabled") { }
    }

    internal sealed class UnlistedGateSideException : ExtendedException
    {
        public UnlistedGateSideException(string sideName, string gateName)
            : base("gate-4", $"Gate side '{sideName}' is not listed in sides of '{gateName}'") { }
    }
}

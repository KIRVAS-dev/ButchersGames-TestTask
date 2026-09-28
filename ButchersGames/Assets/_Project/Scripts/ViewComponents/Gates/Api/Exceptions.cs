using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Gates
{
    internal sealed class MissingGateFieldException : ExtendedException
    {
        internal MissingGateFieldException(string fieldName, string objectName)
            : base("gate-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidGateValueException : ExtendedException
    {
        internal InvalidGateValueException(
            string fieldName,
            string objectName,
            int value)
            : base("gate-2", $"Field '{fieldName}' has invalid value '{value}' on '{objectName}'") { }
    }

    internal sealed class InvalidGateSideColliderException : ExtendedException
    {
        internal InvalidGateSideColliderException(string objectName)
            : base("gate-3", $"Collider on '{objectName}' must have isTrigger enabled") { }
    }

    internal sealed class UnlistedGateSideException : ExtendedException
    {
        internal UnlistedGateSideException(string sideName, string gateName)
            : base("gate-4", $"Gate side '{sideName}' is not listed in sides of '{gateName}'") { }
    }

    internal sealed class UninitializedGateSideException : ExtendedException
    {
        internal UninitializedGateSideException(string objectName)
            : base("gate-5", $"Gate side '{objectName}' was triggered before its gate was initialized") { }
    }
}

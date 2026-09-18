using Infrastructure.ExtendedExceptions;

namespace Input
{
    public sealed class MissingButtonClickTriggerFieldException : ExtendedException
    {
        public MissingButtonClickTriggerFieldException(string fieldName, string objectName)
            : base("button-click-trigger-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

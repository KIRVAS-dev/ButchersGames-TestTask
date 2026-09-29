using Infrastructure.ExtendedExceptions;

namespace UI.RunnerOverlay
{
    public sealed class MissingRunnerOverlayFieldException : ExtendedException
    {
        public MissingRunnerOverlayFieldException(string fieldName, string objectName)
            : base("runner-overlay-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

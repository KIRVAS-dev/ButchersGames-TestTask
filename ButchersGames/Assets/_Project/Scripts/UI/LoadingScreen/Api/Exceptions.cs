using Infrastructure.ExtendedExceptions;

namespace UI.LoadingScreen
{
    internal sealed class MissingLoadingScreenFieldException : ExtendedException
    {
        internal MissingLoadingScreenFieldException(string fieldName, string objectName)
            : base("loading-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidLoadingScreenValueException : ExtendedException
    {
        internal InvalidLoadingScreenValueException(
            string fieldName,
            string objectName,
            int value)
            : base("loading-screen-2", $"Field '{fieldName}' on '{objectName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidLoadingScreenConfigValueException : ExtendedException
    {
        internal InvalidLoadingScreenConfigValueException(string fieldName, float value)
            : base("loading-screen-3", $"Field '{fieldName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidLoadingScreenTextException : ExtendedException
    {
        internal InvalidLoadingScreenTextException(
            string fieldName,
            string objectName,
            int dotCount)
            : base("loading-screen-4", $"Text of '{fieldName}' on '{objectName}' must end with {dotCount} dots") { }
    }
}

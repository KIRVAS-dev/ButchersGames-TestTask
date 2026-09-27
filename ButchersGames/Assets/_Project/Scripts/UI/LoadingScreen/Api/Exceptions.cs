using Infrastructure.ExtendedExceptions;

namespace UI.LoadingScreen
{
    internal sealed class MissingLoadingScreenFieldException : ExtendedException
    {
        public MissingLoadingScreenFieldException(string fieldName, string objectName)
            : base("loading-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidLoadingScreenViewValueException : ExtendedException
    {
        public InvalidLoadingScreenViewValueException(
            string fieldName,
            string objectName,
            int value)
            : base("loading-screen-2", $"Field '{fieldName}' on '{objectName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidLoadingScreenConfigValueException : ExtendedException
    {
        public InvalidLoadingScreenConfigValueException(string fieldName, float value)
            : base("loading-screen-3", $"Field '{fieldName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidLoadingScreenTextException : ExtendedException
    {
        public InvalidLoadingScreenTextException(
            string fieldName,
            string objectName,
            int dotCount)
            : base("loading-screen-4", $"Text of '{fieldName}' on '{objectName}' must end with {dotCount} dots") { }
    }
}

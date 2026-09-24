using Infrastructure.ExtendedExceptions;

namespace UI.StartScreen
{
    internal sealed class MissingStartScreenFieldException : ExtendedException
    {
        public MissingStartScreenFieldException(string fieldName, string objectName)
            : base("start-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidStartScreenValueException : ExtendedException
    {
        public InvalidStartScreenValueException(string fieldName, float value)
            : base("start-screen-2", $"Field '{fieldName}' has invalid value '{value}'") { }
    }
}

using Infrastructure.ExtendedExceptions;

namespace UI.StartScreen
{
    public sealed class MissingStartScreenFieldException : ExtendedException
    {
        public MissingStartScreenFieldException(string fieldName, string objectName)
            : base("start-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace UI.ResultScreen
{
    internal sealed class MissingResultScreenFieldException : ExtendedException
    {
        public MissingResultScreenFieldException(string fieldName, string objectName)
            : base("result-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class UnhandledResultScreenStateException : ExtendedException
    {
        public UnhandledResultScreenStateException(GameState state)
            : base("result-screen-2", $"GameState '{state}' is not handled by the result screen") { }
    }
}

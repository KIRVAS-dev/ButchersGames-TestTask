using ExtendedExceptions;

namespace UI.Hud
{
    public sealed class MissingHudFieldException : ExtendedException
    {
        public MissingHudFieldException(string fieldName, string objectName)
            : base("hud-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

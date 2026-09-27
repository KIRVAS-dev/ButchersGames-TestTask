using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace UI.Hud
{
    internal sealed class MissingHudFieldException : ExtendedException
    {
        internal MissingHudFieldException(string fieldName, string objectName)
            : base("hud-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class UnhandledHudStateException : ExtendedException
    {
        internal UnhandledHudStateException(GameState state)
            : base("hud-2", $"GameState '{state}' is not handled by the HUD presenter") { }
    }
}

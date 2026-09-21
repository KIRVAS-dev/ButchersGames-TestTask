using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;

namespace UI.Hud
{
    internal sealed class MissingHudFieldException : ExtendedException
    {
        public MissingHudFieldException(string fieldName, string objectName)
            : base("hud-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidHudStageAppearanceCountException : ExtendedException
    {
        public InvalidHudStageAppearanceCountException(WealthStage stage, int count)
            : base("hud-2", $"Expected exactly one appearance for stage '{stage}', found {count}") { }
    }

    internal sealed class EmptyHudStageNameException : ExtendedException
    {
        public EmptyHudStageNameException(WealthStage stage)
            : base("hud-3", $"Display name for stage '{stage}' is empty") { }
    }
}

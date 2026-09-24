using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;

namespace UI.WealthIndicator
{
    internal sealed class MissingWealthIndicatorFieldException : ExtendedException
    {
        public MissingWealthIndicatorFieldException(string fieldName, string objectName)
            : base("wealth-indicator-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidWealthIndicatorStageAppearanceCountException : ExtendedException
    {
        public InvalidWealthIndicatorStageAppearanceCountException(WealthStage stage, int count)
            : base("wealth-indicator-2", $"Expected exactly one appearance for stage '{stage}', found {count}") { }
    }

    internal sealed class EmptyWealthIndicatorStageNameException : ExtendedException
    {
        public EmptyWealthIndicatorStageNameException(WealthStage stage)
            : base("wealth-indicator-3", $"Display name for stage '{stage}' is empty") { }
    }
}

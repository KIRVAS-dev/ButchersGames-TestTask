using Infrastructure.ExtendedExceptions;

namespace ViewComponents.WealthMeter
{
    internal sealed class MissingCharacterAppearanceViewFieldException : ExtendedException
    {
        internal MissingCharacterAppearanceViewFieldException(string fieldName, string objectName)
            : base("wealth-meter-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class MissingWealthParticlesViewFieldException : ExtendedException
    {
        internal MissingWealthParticlesViewFieldException(string fieldName, string objectName)
            : base("wealth-meter-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

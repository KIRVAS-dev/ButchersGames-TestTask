using Infrastructure.ExtendedExceptions;

namespace ViewComponents.WealthMeter
{
    public sealed class MissingCharacterAppearanceViewFieldException : ExtendedException
    {
        public MissingCharacterAppearanceViewFieldException(string fieldName, string objectName)
            : base("character-appearance-view-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

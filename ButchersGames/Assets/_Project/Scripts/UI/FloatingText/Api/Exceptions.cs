using Infrastructure.ExtendedExceptions;

namespace UI.FloatingText
{
    internal sealed class MissingFloatingTextFieldException : ExtendedException
    {
        public MissingFloatingTextFieldException(string fieldName, string objectName)
            : base("floating-text-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidFloatingTextValueException : ExtendedException
    {
        public InvalidFloatingTextValueException(string fieldName, float value)
            : base("floating-text-2", $"Field '{fieldName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidFloatingTextTimingException : ExtendedException
    {
        public InvalidFloatingTextTimingException(
            float lifetime,
            float appearDuration,
            float disappearDuration)
            : base(
                "floating-text-3",
                $"Appear duration '{appearDuration}' plus disappear duration '{disappearDuration}' exceeds lifetime '{lifetime}'"
            ) { }
    }
}

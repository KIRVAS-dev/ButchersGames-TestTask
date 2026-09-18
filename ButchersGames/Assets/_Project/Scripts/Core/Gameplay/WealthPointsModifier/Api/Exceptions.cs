using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.WealthPointsModifier
{
    public sealed class InvalidWealthPointsModifierValueException : ExtendedException
    {
        public InvalidWealthPointsModifierValueException(string fieldName, int value)
            : base("wealthpointsmodifier-1", $"WealthPointsModifierConfig field '{fieldName}' has invalid value {value}") { }
    }

    public sealed class InvalidWealthPointsModifierTypeException : ExtendedException
    {
        public InvalidWealthPointsModifierTypeException(WealthPointsModifierType type)
            : base("wealthpointsmodifier-2", $"Unsupported WealthPointsModifierType value {type}") { }
    }
}

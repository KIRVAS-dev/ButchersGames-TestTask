using Infrastructure.ExtendedExceptions;

namespace ViewComponents.WealthPointsModifier
{
    internal sealed class MissingWealthPointsModifierConfigException : ExtendedException
    {
        public MissingWealthPointsModifierConfigException(string fieldName, string objectName)
            : base("wealthpointsmodifier-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class InvalidWealthPointsModifierColliderException : ExtendedException
    {
        public InvalidWealthPointsModifierColliderException(string objectName)
            : base("wealthpointsmodifier-2", $"Collider on {objectName} must have isTrigger enabled") { }
    }
}

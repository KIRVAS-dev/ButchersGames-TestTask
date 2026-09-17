using ExtendedExceptions;

namespace ViewComponents.WealthPointsModifier
{
    public sealed class MissingWealthPointsModifierConfigException : ExtendedException
    {
        public MissingWealthPointsModifierConfigException(string fieldName, string objectName)
            : base("wealthpointsmodifier-1", $"Missing field {fieldName} on {objectName}") { }
    }

    public sealed class InvalidWealthPointsModifierColliderException : ExtendedException
    {
        public InvalidWealthPointsModifierColliderException(string objectName)
            : base("wealthpointsmodifier-2", $"Collider on {objectName} must have isTrigger enabled") { }
    }
}

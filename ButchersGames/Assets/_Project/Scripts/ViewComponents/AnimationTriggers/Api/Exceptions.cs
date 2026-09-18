using ExtendedExceptions;

namespace ViewComponents.AnimationTriggers
{
    public sealed class MissingAnimationTriggerZoneFieldException : ExtendedException
    {
        public MissingAnimationTriggerZoneFieldException(string fieldName, string objectName)
            : base("animation-trigger-1", $"Missing field {fieldName} on {objectName}") { }
    }

    public sealed class InvalidAnimationTriggerZoneColliderException : ExtendedException
    {
        public InvalidAnimationTriggerZoneColliderException(string objectName)
            : base("animation-trigger-2", $"Collider on {objectName} must have isTrigger enabled") { }
    }
}

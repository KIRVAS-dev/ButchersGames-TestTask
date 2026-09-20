using Infrastructure.ExtendedExceptions;

namespace ViewComponents.LaneBarriers
{
    public sealed class InvalidLaneBarrierColliderException : ExtendedException
    {
        public InvalidLaneBarrierColliderException(string objectName)
            : base("lane-barrier-1", $"Collider on {objectName} must have isTrigger enabled") { }
    }

    public sealed class MissingLaneBarrierFieldException : ExtendedException
    {
        public MissingLaneBarrierFieldException(string fieldName, string objectName)
            : base("lane-barrier-2", $"Missing field {fieldName} on {objectName}") { }
    }
}

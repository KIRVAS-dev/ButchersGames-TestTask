using Infrastructure.ExtendedExceptions;

namespace ViewComponents.LaneBarriers
{
    internal sealed class InvalidLaneBarrierColliderException : ExtendedException
    {
        internal InvalidLaneBarrierColliderException(string objectName)
            : base("lane-barrier-1", $"Collider on {objectName} must have isTrigger enabled") { }
    }

    internal sealed class MissingLaneBarrierFieldException : ExtendedException
    {
        internal MissingLaneBarrierFieldException(string fieldName, string objectName)
            : base("lane-barrier-2", $"Missing field {fieldName} on {objectName}") { }
    }
}

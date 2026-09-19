using Infrastructure.ExtendedExceptions;

namespace ViewComponents.LaneBarriers
{
    public sealed class InvalidLaneBarrierColliderException : ExtendedException
    {
        public InvalidLaneBarrierColliderException(string objectName)
            : base("lane-barrier-1", $"Collider on {objectName} must have isTrigger enabled") { }
    }
}

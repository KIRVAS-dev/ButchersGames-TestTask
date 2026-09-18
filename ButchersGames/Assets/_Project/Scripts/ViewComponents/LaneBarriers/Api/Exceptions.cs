using ExtendedExceptions;

namespace ViewComponents.LaneBarriers
{
    public sealed class InvalidLaneBarrierColliderException : ExtendedException
    {
        public InvalidLaneBarrierColliderException(string objectName)
            : base("lane-barrier-1", $"Collider on {objectName} must have isTrigger enabled") { }
    }

    public sealed class MissingLaneBarrierSplineContainerException : ExtendedException
    {
        public MissingLaneBarrierSplineContainerException(string objectName)
            : base("lane-barrier-2", $"No SplineContainer found in scene to calculate lateral range for {objectName}") { }
    }
}

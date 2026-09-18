using Infrastructure.ExtendedExceptions;

namespace ViewComponents.RunnerMovement
{
    public sealed class MissingSplineContainerException : ExtendedException
    {
        public MissingSplineContainerException(string objectName)
            : base("runner-movement-view-1", $"No active SplineContainer found in loaded level for {objectName}") { }
    }

    public sealed class MissingRunnerMovementViewFieldException : ExtendedException
    {
        public MissingRunnerMovementViewFieldException(string fieldName, string objectName)
            : base("runner-movement-view-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidSplineLengthException : ExtendedException
    {
        public InvalidSplineLengthException(string objectName, float length)
            : base("runner-movement-view-3", $"SplineContainer length {length} is invalid for {objectName}") { }
    }

    public sealed class InvalidSplineTangentException : ExtendedException
    {
        public InvalidSplineTangentException(string objectName)
            : base("runner-movement-view-4", $"SplineContainer tangent is zero or invalid for {objectName}") { }
    }
}

using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Track
{
    public sealed class MissingSplineContainerException : ExtendedException
    {
        public MissingSplineContainerException(string objectName)
            : base("track-1", $"No active SplineContainer found in loaded level for {objectName}") { }
    }

    public sealed class InvalidSplineLengthException : ExtendedException
    {
        public InvalidSplineLengthException(string objectName, float length)
            : base("track-2", $"SplineContainer length {length} is invalid for {objectName}") { }
    }

    public sealed class InvalidSplineTangentException : ExtendedException
    {
        public InvalidSplineTangentException(string objectName)
            : base("track-3", $"SplineContainer tangent is zero or invalid for {objectName}") { }
    }
}

using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Track
{
    internal sealed class InvalidSplineLengthException : ExtendedException
    {
        internal InvalidSplineLengthException(string objectName, float length)
            : base("track-2", $"SplineContainer length {length} is invalid for {objectName}") { }
    }

    internal sealed class InvalidSplineTangentException : ExtendedException
    {
        internal InvalidSplineTangentException(string objectName)
            : base("track-3", $"SplineContainer tangent is zero or invalid for {objectName}") { }
    }
}

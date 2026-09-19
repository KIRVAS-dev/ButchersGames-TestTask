using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Finish
{
    public sealed class MissingFinishMarkerException : ExtendedException
    {
        public MissingFinishMarkerException(string objectName)
            : base("finish-1", $"No active FinishMarker found in loaded level for {objectName}") { }
    }
}

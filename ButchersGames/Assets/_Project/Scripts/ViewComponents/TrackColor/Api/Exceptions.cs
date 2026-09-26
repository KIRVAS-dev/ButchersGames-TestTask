using Infrastructure.ExtendedExceptions;

namespace ViewComponents.TrackColor
{
    internal sealed class EmptyTrackPaletteException : ExtendedException
    {
        public EmptyTrackPaletteException(string configName)
            : base("track-color-1", $"Track palette {configName} is empty") { }
    }

    internal sealed class MissingTrackColorFieldException : ExtendedException
    {
        public MissingTrackColorFieldException(string fieldName, string objectName)
            : base("track-color-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}

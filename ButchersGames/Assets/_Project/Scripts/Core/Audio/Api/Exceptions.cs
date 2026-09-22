using Infrastructure.ExtendedExceptions;

namespace Core.Audio
{
    public sealed class MissingStudioListenerAnchorTransformException : ExtendedException
    {
        public MissingStudioListenerAnchorTransformException()
            : base("studio-listener-1", "StudioListenerAnchor transform is not assigned") { }
    }
}

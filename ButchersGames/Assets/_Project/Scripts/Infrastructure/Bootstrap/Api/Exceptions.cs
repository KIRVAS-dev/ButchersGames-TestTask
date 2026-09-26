using Infrastructure.ExtendedExceptions;

namespace Infrastructure.Bootstrap
{
    internal sealed class MissingStudioListenerAnchorTransformException : ExtendedException
    {
        internal MissingStudioListenerAnchorTransformException()
            : base("studio-listener-1", "StudioListenerAnchor transform is not assigned") { }
    }
}

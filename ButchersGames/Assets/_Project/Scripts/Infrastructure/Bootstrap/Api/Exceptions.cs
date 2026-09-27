using Infrastructure.ExtendedExceptions;

namespace Infrastructure.Bootstrap
{
    internal sealed class MissingStudioListenerAnchorTransformException : ExtendedException
    {
        internal MissingStudioListenerAnchorTransformException()
            : base("studio-listener-1", "StudioListenerAnchor transform is not assigned") { }
    }

    internal sealed class MissingLoadingScreenViewException : ExtendedException
    {
        internal MissingLoadingScreenViewException()
            : base("loading-screen-view-1", "LoadingScreenView is not assigned on ProjectScope") { }
    }
}

using Infrastructure.ExtendedExceptions;

namespace UI.SafeArea
{
    internal sealed class SafeAreaRectMissingRectTransformException : ExtendedException
    {
        public SafeAreaRectMissingRectTransformException()
            : base("safe-area-1", "RectTransform is missing. RequireComponent should ensure it exists.") { }
    }
}

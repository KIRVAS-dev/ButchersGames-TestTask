using ExtendedExceptions;

namespace UI.SafeArea
{
    public sealed class SafeAreaRectMissingRectTransformException : ExtendedException
    {
        public SafeAreaRectMissingRectTransformException()
            : base("safe-area-1", "RectTransform is missing. RequireComponent should ensure it exists.") { }
    }
}

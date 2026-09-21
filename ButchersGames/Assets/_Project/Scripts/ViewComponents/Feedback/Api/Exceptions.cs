using Core.Gameplay.Feedback;
using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Feedback
{
    internal sealed class UnhandledFeedbackStateException : ExtendedException
    {
        public UnhandledFeedbackStateException(GameState state)
            : base("feedback-1", $"GameState '{state}' is not handled by the feedback presenter") { }
    }

    internal sealed class DuplicateFeedbackEntryException : ExtendedException
    {
        public DuplicateFeedbackEntryException(FeedbackType type, string objectName)
            : base("feedback-2", $"Feedback entry for '{type}' is duplicated on '{objectName}'") { }
    }
}

namespace Core.Gameplay.Feedback
{
    public interface IFeedbackPerformer
    {
        void Play(FeedbackType type);
    }
}

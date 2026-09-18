namespace Input
{
    public interface IResultScreenInput
    {
        ITrigger RetryTrigger { get; }
        ITrigger NextTrigger { get; }
    }
}

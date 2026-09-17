using R3;

namespace Core
{
    public interface IGameplayInputBlock
    {
        ReadOnlyReactiveProperty<bool> IsBlocked { get; }
        void Block();
        void Unblock();
    }
}

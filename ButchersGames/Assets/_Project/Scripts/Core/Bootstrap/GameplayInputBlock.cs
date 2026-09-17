using R3;

namespace Core.Bootstrap
{
    public sealed class GameplayInputBlock : IGameplayInputBlock
    {
        private readonly ReactiveProperty<bool> _isBlocked = new(false);

        public ReadOnlyReactiveProperty<bool> IsBlocked => _isBlocked;

        public void Block() => _isBlocked.Value = true;
        public void Unblock() => _isBlocked.Value = false;
    }
}

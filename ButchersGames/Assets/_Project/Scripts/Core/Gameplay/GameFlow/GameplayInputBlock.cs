using System;
using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameplayInputBlock
        : IGameplayInputBlock,
          IDisposable
    {
        private readonly ReadOnlyReactiveProperty<bool> _isBlocked;

        public GameplayInputBlock(GameStateModel model)
        {
            _isBlocked = model.State.Select(state => state != GameState.Run).ToReadOnlyReactiveProperty();
        }

        ReadOnlyReactiveProperty<bool> IGameplayInputBlock.IsBlocked => _isBlocked;

        void IDisposable.Dispose()
        {
            _isBlocked.Dispose();
        }
    }
}

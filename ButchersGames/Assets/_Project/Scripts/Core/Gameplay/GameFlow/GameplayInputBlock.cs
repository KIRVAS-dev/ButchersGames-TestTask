using System;
using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameplayInputBlock
        : IGameplayInputBlock,
          IDisposable
    {
        public GameplayInputBlock(GameStateModel model)
        {
            IsBlocked = model.State.Select(state => state != GameState.Run).ToReadOnlyReactiveProperty();
        }

        public ReadOnlyReactiveProperty<bool> IsBlocked { get; }

        void IDisposable.Dispose()
        {
            IsBlocked.Dispose();
        }
    }
}

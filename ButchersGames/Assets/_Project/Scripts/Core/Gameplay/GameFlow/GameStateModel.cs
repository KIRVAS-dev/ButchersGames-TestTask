using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameStateModel
    {
        public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>(GameState.Tutorial);
    }
}

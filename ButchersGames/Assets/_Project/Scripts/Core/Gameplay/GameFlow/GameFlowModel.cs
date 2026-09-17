using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameFlowModel
    {
        public ReactiveProperty<GameFlowState> State { get; } = new ReactiveProperty<GameFlowState>(GameFlowState.WaitingToStart);
    }
}

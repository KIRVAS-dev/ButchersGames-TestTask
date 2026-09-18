using R3;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementModel
    {
        public float LateralOffset { get; set; }
        public ReactiveProperty<RunnerMovementState> State { get; } =
            new ReactiveProperty<RunnerMovementState>(RunnerMovementState.Moving);
    }
}

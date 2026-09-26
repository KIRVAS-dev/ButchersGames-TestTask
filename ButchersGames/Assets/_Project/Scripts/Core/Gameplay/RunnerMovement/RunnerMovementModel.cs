using R3;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementModel : IReadOnlyRunnerMovementModel
    {
        public ReactiveProperty<float> CurrentRunnerCoordinate { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<float> LateralOffset { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<RunnerLateralDirection> LateralDirection { get; } =
            new ReactiveProperty<RunnerLateralDirection>(RunnerLateralDirection.None);
        public ReactiveProperty<RunnerMovementState> State { get; } =
            new ReactiveProperty<RunnerMovementState>(RunnerMovementState.Moving);

        ReadOnlyReactiveProperty<float> IReadOnlyRunnerMovementModel.CurrentRunnerCoordinate => CurrentRunnerCoordinate;
        ReadOnlyReactiveProperty<float> IReadOnlyRunnerMovementModel.LateralOffset => LateralOffset;
        ReadOnlyReactiveProperty<RunnerLateralDirection> IReadOnlyRunnerMovementModel.LateralDirection => LateralDirection;
        ReadOnlyReactiveProperty<RunnerMovementState> IReadOnlyRunnerMovementModel.State => State;
    }
}

using R3;

namespace Core.Gameplay.RunnerMovement
{
    public interface IReadOnlyRunnerMovementModel
    {
        ReadOnlyReactiveProperty<float> CurrentRunnerCoordinate { get; }
        ReadOnlyReactiveProperty<float> LateralOffset { get; }
        ReadOnlyReactiveProperty<RunnerLateralDirection> LateralDirection { get; }
        ReadOnlyReactiveProperty<RunnerMovementState> State { get; }
    }
}

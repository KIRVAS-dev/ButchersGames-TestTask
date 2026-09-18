using Core.Gameplay.RunnerMovement;

namespace ViewComponents.RunnerMovement
{
    public interface IRunnerTrackFollowerView
    {
        void SetLateralOffset(float value);
        void SetMovementState(RunnerMovementState state);
    }
}

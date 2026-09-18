namespace Core.Gameplay.RunnerMovement
{
    public interface IRunnerMovementSettings
    {
        float ForwardSpeed { get; }
        float TrackHalfWidth { get; }
        float LateralCorrectionSpeed { get; }
    }
}

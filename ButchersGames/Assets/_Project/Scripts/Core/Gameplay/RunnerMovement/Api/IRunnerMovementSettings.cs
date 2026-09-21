namespace Core.Gameplay.RunnerMovement
{
    public interface IRunnerMovementSettings
    {
        float ForwardSpeed { get; }
        float LateralRange { get; }
        float LateralCorrectionSpeed { get; }
    }
}

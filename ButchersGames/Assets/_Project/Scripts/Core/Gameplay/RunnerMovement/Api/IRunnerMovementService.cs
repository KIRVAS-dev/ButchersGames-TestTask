namespace Core.Gameplay.RunnerMovement
{
    public interface IRunnerMovementService
    {
        void SetNormalizedLateralOffset(float normalizedOffset);
        void AdvanceLateralCorrections(float deltaTime);
    }
}

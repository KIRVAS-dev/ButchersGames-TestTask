using System;
using Core.Gameplay.LevelProgression;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementService
        : IRunnerMovementService,
          IDisposable
    {
        private const float NormalizedLateralOffsetMin = -1f;
        private const float NormalizedLateralOffsetMax = 1f;
        private const float CenteredLateralOffset = 0f;

        private readonly ILevelProvider _levelProvider;
        private readonly IRunnerMovementSettings _settings;
        private readonly RunnerMovementModel _model;

        public RunnerMovementService(
            ILevelProvider levelProvider,
            IRunnerMovementSettings settings,
            RunnerMovementModel model)
        {
            _levelProvider = levelProvider;
            _settings = settings;
            _model = model;

            _levelProvider.LevelLoaded += ResetLateralOffset;

            ResetLateralOffset();
        }

        public void SetNormalizedLateralOffset(float normalizedOffset)
        {
            float clampedNormalizedOffset = Math.Clamp(normalizedOffset, NormalizedLateralOffsetMin, NormalizedLateralOffsetMax);
            _model.LateralOffset = clampedNormalizedOffset * _settings.TrackHalfWidth;
        }

        void IDisposable.Dispose()
        {
            _levelProvider.LevelLoaded -= ResetLateralOffset;
        }

        private void ResetLateralOffset()
        {
            _model.LateralOffset = CenteredLateralOffset;
        }
    }
}

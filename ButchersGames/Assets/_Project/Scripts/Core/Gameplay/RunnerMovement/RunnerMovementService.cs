using System;
using System.Collections.Generic;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementService
        : IRunnerMovementService,
          IDisposable
    {
        private const float NormalizedLateralOffsetMin = -1f;
        private const float NormalizedLateralOffsetMax = 1f;
        private const float CenteredLateralOffset = 0f;
        private const int NoActiveObstacles = 0;

        private readonly ILevelProvider _levelProvider;
        private readonly IRunnerMovementSettings _settings;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly RunnerMovementModel _model;
        private readonly List<IObstacle> _subscribedObstacles = new List<IObstacle>();
        private int _activeObstacleCount;

        public RunnerMovementService(
            ILevelProvider levelProvider,
            IRunnerMovementSettings settings,
            IObstacleRegistry obstacleRegistry,
            RunnerMovementModel model)
        {
            _levelProvider = levelProvider;
            _settings = settings;
            _obstacleRegistry = obstacleRegistry;
            _model = model;

            _levelProvider.LevelLoaded += OnLevelLoaded;
            _obstacleRegistry.ObstaclesChanged += ResubscribeToObstacles;

            OnLevelLoaded();
            ResubscribeToObstacles();
        }

        public void SetNormalizedLateralOffset(float normalizedOffset)
        {
            float clampedNormalizedOffset = Math.Clamp(normalizedOffset, NormalizedLateralOffsetMin, NormalizedLateralOffsetMax);
            _model.LateralOffset = clampedNormalizedOffset * _settings.TrackHalfWidth;
        }

        void IDisposable.Dispose()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;
            _obstacleRegistry.ObstaclesChanged -= ResubscribeToObstacles;

            UnsubscribeAllObstacles();
        }

        private void OnLevelLoaded()
        {
            _model.LateralOffset = CenteredLateralOffset;
            _activeObstacleCount = NoActiveObstacles;
            _model.State.Value = RunnerMovementState.Moving;
        }

        private void ResubscribeToObstacles()
        {
            UnsubscribeAllObstacles();

            _subscribedObstacles.AddRange(_obstacleRegistry.Obstacles);

            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Hit += OnObstacleHit;
                obstacle.Released += OnObstacleReleased;
            }
        }

        private void UnsubscribeAllObstacles()
        {
            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Hit -= OnObstacleHit;
                obstacle.Released -= OnObstacleReleased;
            }

            _subscribedObstacles.Clear();
        }

        private void OnObstacleHit()
        {
            _activeObstacleCount++;
            _model.State.Value = RunnerMovementState.Stopped;
        }

        private void OnObstacleReleased()
        {
            _activeObstacleCount--;

            if (_activeObstacleCount <= NoActiveObstacles)
            {
                _activeObstacleCount = NoActiveObstacles;
                _model.State.Value = RunnerMovementState.Moving;
            }
        }
    }
}

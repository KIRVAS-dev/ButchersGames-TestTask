using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using UnityEngine;

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
        private readonly ILaneBarrierRegistry _laneBarrierRegistry;
        private readonly RunnerMovementModel _model;
        private readonly List<IObstacle> _subscribedObstacles = new List<IObstacle>();
        private readonly List<ILaneBarrier> _subscribedLaneBarriers = new List<ILaneBarrier>();
        private readonly Dictionary<ILaneBarrier, LateralClamp> _activeLaneBarrierClamps =
            new Dictionary<ILaneBarrier, LateralClamp>();

        private ILaneBarrier _correctingLaneBarrier;
        private int _activeObstacleCount;
        private float _lateralCorrectionTarget;

        private enum LateralClampDirection
        {
            UpperBound = 0,
            LowerBound = 1
        }

        private readonly struct LateralClamp
        {
            public LateralClamp(LateralClampDirection direction, float boundary)
            {
                Direction = direction;
                Boundary = boundary;
            }

            public LateralClampDirection Direction { get; }
            public float Boundary { get; }
        }

        public RunnerMovementService(
            ILevelProvider levelProvider,
            IRunnerMovementSettings settings,
            IObstacleRegistry obstacleRegistry,
            ILaneBarrierRegistry laneBarrierRegistry,
            RunnerMovementModel model)
        {
            _levelProvider = levelProvider;
            _settings = settings;
            _obstacleRegistry = obstacleRegistry;
            _laneBarrierRegistry = laneBarrierRegistry;
            _model = model;

            _levelProvider.LevelLoaded += OnLevelLoaded;
            _obstacleRegistry.ObstaclesChanged += ResubscribeToObstacles;
            _laneBarrierRegistry.BarriersChanged += ResubscribeToLaneBarriers;

            OnLevelLoaded();
            ResubscribeToObstacles();
            ResubscribeToLaneBarriers();
        }

        void IDisposable.Dispose()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;
            _obstacleRegistry.ObstaclesChanged -= ResubscribeToObstacles;
            _laneBarrierRegistry.BarriersChanged -= ResubscribeToLaneBarriers;

            UnsubscribeAllObstacles();
            UnsubscribeAllLaneBarriers();
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

        private void ResubscribeToLaneBarriers()
        {
            UnsubscribeAllLaneBarriers();

            _subscribedLaneBarriers.AddRange(_laneBarrierRegistry.Barriers);

            foreach (ILaneBarrier barrier in _subscribedLaneBarriers)
            {
                barrier.Entered += OnLaneBarrierEntered;
                barrier.Exited += OnLaneBarrierExited;
            }
        }

        private void UnsubscribeAllLaneBarriers()
        {
            foreach (ILaneBarrier barrier in _subscribedLaneBarriers)
            {
                barrier.Entered -= OnLaneBarrierEntered;
                barrier.Exited -= OnLaneBarrierExited;
            }

            _subscribedLaneBarriers.Clear();
        }

        public void SetNormalizedLateralOffset(float normalizedOffset)
        {
            if (_correctingLaneBarrier != null)
            {
                return;
            }

            float clampedNormalizedOffset = Math.Clamp(normalizedOffset, NormalizedLateralOffsetMin, NormalizedLateralOffsetMax);
            float offset = clampedNormalizedOffset * _settings.TrackHalfWidth;

            foreach (LateralClamp clamp in _activeLaneBarrierClamps.Values)
            {
                offset = ApplyClamp(offset, clamp);
            }

            _model.LateralOffset = offset;
        }

        public void AdvanceLateralCorrections(float deltaTime)
        {
            if (_correctingLaneBarrier == null)
            {
                return;
            }

            float maxStep = _settings.LateralCorrectionSpeed * deltaTime;
            _model.LateralOffset = MoveTowards(_model.LateralOffset, _lateralCorrectionTarget, maxStep);

            if (Mathf.Approximately(_model.LateralOffset, _lateralCorrectionTarget))
            {
                _correctingLaneBarrier = null;
            }
        }

        private void OnLevelLoaded()
        {
            _model.LateralOffset = CenteredLateralOffset;
            _activeObstacleCount = NoActiveObstacles;
            _model.State.Value = RunnerMovementState.Moving;

            _activeLaneBarrierClamps.Clear();
            _correctingLaneBarrier = null;
        }

        private void OnObstacleHit()
        {
            _activeObstacleCount++;
            _model.State.Value = RunnerMovementState.Stopped;
        }

        private void OnObstacleReleased()
        {
            _activeObstacleCount--;

            if (_activeObstacleCount > NoActiveObstacles)
            {
                return;
            }

            _activeObstacleCount = NoActiveObstacles;
            _model.State.Value = RunnerMovementState.Moving;
        }

        private void OnLaneBarrierEntered(ILaneBarrier barrier)
        {
            float currentOffset = _model.LateralOffset;

            if (currentOffset <= barrier.MinLateralOffset)
            {
                _activeLaneBarrierClamps[barrier] = new LateralClamp(LateralClampDirection.UpperBound, barrier.MinLateralOffset);
                return;
            }

            if (currentOffset >= barrier.MaxLateralOffset)
            {
                _activeLaneBarrierClamps[barrier] = new LateralClamp(LateralClampDirection.LowerBound, barrier.MaxLateralOffset);
                return;
            }

            float distanceToMin = currentOffset - barrier.MinLateralOffset;
            float distanceToMax = barrier.MaxLateralOffset - currentOffset;

            LateralClamp clamp = distanceToMin <= distanceToMax
                ? new LateralClamp(LateralClampDirection.UpperBound, barrier.MinLateralOffset)
                : new LateralClamp(LateralClampDirection.LowerBound, barrier.MaxLateralOffset);

            _activeLaneBarrierClamps[barrier] = clamp;

            _correctingLaneBarrier = barrier;
            _lateralCorrectionTarget = clamp.Boundary;
        }

        private void OnLaneBarrierExited(ILaneBarrier barrier)
        {
            _activeLaneBarrierClamps.Remove(barrier);

            if (_correctingLaneBarrier == barrier)
            {
                _correctingLaneBarrier = null;
            }
        }

        private float ApplyClamp(float offset, LateralClamp clamp)
        {
            return clamp.Direction == LateralClampDirection.UpperBound
                ? Math.Min(offset, clamp.Boundary)
                : Math.Max(offset, clamp.Boundary);
        }

        private float MoveTowards(
            float current,
            float target,
            float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}

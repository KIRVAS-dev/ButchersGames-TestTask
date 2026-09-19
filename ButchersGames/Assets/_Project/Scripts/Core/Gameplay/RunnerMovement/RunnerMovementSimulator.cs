using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementSimulator
    {
        private const float NormalizedLateralOffsetMin = -1f;
        private const float NormalizedLateralOffsetMax = 1f;
        private const float StartDistance = 0f;
        private const float CenteredLateralOffset = 0f;
        private const int NoActiveObstacles = 0;
        private const float LateralOffsetEpsilon = 0.0001f;

        private readonly IRunnerMovementSettings _settings;
        private readonly RunnerMovementModel _model;
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

        public RunnerMovementSimulator(IRunnerMovementSettings settings, RunnerMovementModel model)
        {
            _settings = settings;
            _model = model;
        }

        public void Reset()
        {
            _model.DistanceTraveled.Value = StartDistance;
            _model.LateralOffset.Value = CenteredLateralOffset;
            _model.State.Value = RunnerMovementState.Moving;

            _activeObstacleCount = NoActiveObstacles;
            _activeLaneBarrierClamps.Clear();
            _correctingLaneBarrier = null;
        }

        public void Tick(float deltaTime, float trackLength)
        {
            if (_model.State.Value != RunnerMovementState.Moving)
            {
                return;
            }

            AdvanceDistance(deltaTime, trackLength);
            AdvanceLateralCorrection(deltaTime);
        }

        public void AddNormalizedLateralOffsetDelta(float normalizedDelta)
        {
            float currentNormalizedOffset = _model.LateralOffset.Value / _settings.TrackHalfWidth;

            SetNormalizedLateralOffset(currentNormalizedOffset + normalizedDelta);
        }

        private void SetNormalizedLateralOffset(float normalizedOffset)
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

            _model.LateralOffset.Value = offset;
        }

        public void HitObstacle()
        {
            _activeObstacleCount++;
            _model.State.Value = RunnerMovementState.Stopped;
        }

        public void ReleaseObstacle()
        {
            _activeObstacleCount--;

            if (_activeObstacleCount > NoActiveObstacles)
            {
                return;
            }

            _activeObstacleCount = NoActiveObstacles;
            _model.State.Value = RunnerMovementState.Moving;
        }

        public void EnterLaneBarrier(ILaneBarrier barrier)
        {
            float currentOffset = _model.LateralOffset.Value;

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

        public void ExitLaneBarrier(ILaneBarrier barrier)
        {
            _activeLaneBarrierClamps.Remove(barrier);

            if (_correctingLaneBarrier == barrier)
            {
                _correctingLaneBarrier = null;
            }
        }

        private void AdvanceDistance(float deltaTime, float trackLength)
        {
            float distance = _model.DistanceTraveled.Value + _settings.ForwardSpeed * deltaTime;

            _model.DistanceTraveled.Value = Math.Min(distance, trackLength);
        }

        private void AdvanceLateralCorrection(float deltaTime)
        {
            if (_correctingLaneBarrier == null)
            {
                return;
            }

            float maxStep = _settings.LateralCorrectionSpeed * deltaTime;
            _model.LateralOffset.Value = MoveTowards(_model.LateralOffset.Value, _lateralCorrectionTarget, maxStep);

            if (Math.Abs(_model.LateralOffset.Value - _lateralCorrectionTarget) < LateralOffsetEpsilon)
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

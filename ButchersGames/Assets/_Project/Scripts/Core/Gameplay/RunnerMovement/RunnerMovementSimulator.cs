using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.RunnerBody;
using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.RunnerMovement
{
    internal sealed class RunnerMovementSimulator
    {
        private const float NormalizedLateralOffsetMin = -1f;
        private const float NormalizedLateralOffsetMax = 1f;
        private const float CenteredLateralOffset = 0f;
        private const int NoActiveObstacles = 0;
        private const float LateralOffsetEpsilon = 0.0001f;
        private const float HalfFactor = 0.5f;

        private readonly IRunnerMovementSettings _settings;
        private readonly IRunnerBodyProvider _runnerBodyProvider;
        private readonly RunnerMovementModel _model;
        private readonly Dictionary<ILaneBarrier, LateralClamp> _activeLaneBarrierClamps =
            new Dictionary<ILaneBarrier, LateralClamp>();
        private readonly float _lateralHalfRange;

        private ILaneBarrier _correctingLaneBarrier;
        private int _activeObstacleCount;
        private float _lateralCorrectionTarget;
        private float _previousLateralOffset;

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

        internal RunnerMovementSimulator(
            IRunnerMovementSettings settings,
            IRunnerBodyProvider runnerBodyProvider,
            RunnerMovementModel model)
        {
            _settings = settings;
            _runnerBodyProvider = runnerBodyProvider;
            _model = model;
            _lateralHalfRange = settings.LateralRange * HalfFactor;
        }

        internal void Tick(float deltaTime, float finishCoordinate)
        {
            if (_model.State.Value == RunnerMovementState.Moving)
            {
                MoveForward(deltaTime, finishCoordinate);
                CorrectLateralOffset(deltaTime);
            }

            UpdateLateralDirection();
        }

        internal void Reset(float startCoordinate)
        {
            _model.CurrentRunnerCoordinate.Value = startCoordinate;
            _model.LateralOffset.Value = CenteredLateralOffset;
            _model.LateralDirection.Value = RunnerLateralDirection.None;
            _model.State.Value = RunnerMovementState.Moving;

            _previousLateralOffset = CenteredLateralOffset;
            _activeObstacleCount = NoActiveObstacles;
            _activeLaneBarrierClamps.Clear();
            _correctingLaneBarrier = null;
        }

        internal void AddNormalizedLateralOffsetDelta(float normalizedDelta)
        {
            if (_model.State.Value != RunnerMovementState.Moving)
            {
                return;
            }

            float currentNormalizedOffset = _model.LateralOffset.Value / _lateralHalfRange;

            SetNormalizedLateralOffset(currentNormalizedOffset + normalizedDelta);
        }

        internal void HitObstacle()
        {
            _activeObstacleCount++;
            _model.State.Value = RunnerMovementState.Stopped;
        }

        internal void ReleaseObstacle()
        {
            _activeObstacleCount--;

            Guard.AgainstLessThan(
                _activeObstacleCount,
                NoActiveObstacles,
                () => new UnmatchedRunnerObstacleReleaseException(_activeObstacleCount)
            );

            if (_activeObstacleCount > NoActiveObstacles)
            {
                return;
            }

            _model.State.Value = RunnerMovementState.Moving;
        }

        internal void EnterLaneBarrier(ILaneBarrier barrier)
        {
            float halfBodyWidth = _runnerBodyProvider.Width * HalfFactor;

            LateralClamp leftSideClamp = new LateralClamp(
                LateralClampDirection.UpperBound,
                barrier.MinLateralOffset - halfBodyWidth
            );

            LateralClamp rightSideClamp = new LateralClamp(
                LateralClampDirection.LowerBound,
                barrier.MaxLateralOffset + halfBodyWidth
            );

            float currentOffset = _model.LateralOffset.Value;

            if (currentOffset <= leftSideClamp.Boundary)
            {
                _activeLaneBarrierClamps[barrier] = leftSideClamp;
                return;
            }

            if (currentOffset >= rightSideClamp.Boundary)
            {
                _activeLaneBarrierClamps[barrier] = rightSideClamp;
                return;
            }

            LateralClamp clamp = _model.LateralDirection.Value switch
            {
                RunnerLateralDirection.Right => leftSideClamp,
                RunnerLateralDirection.Left => rightSideClamp,
                RunnerLateralDirection.None => NearestClamp(currentOffset, leftSideClamp, rightSideClamp),
                _ => throw new UnhandledRunnerLateralDirectionException(_model.LateralDirection.Value)
            };

            _activeLaneBarrierClamps[barrier] = clamp;

            _correctingLaneBarrier = barrier;
            _lateralCorrectionTarget = clamp.Boundary;
        }

        internal void ExitLaneBarrier(ILaneBarrier barrier)
        {
            _activeLaneBarrierClamps.Remove(barrier);

            if (_correctingLaneBarrier == barrier)
            {
                _correctingLaneBarrier = null;
            }
        }

        private static float MoveTowards(
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

        private static LateralClamp NearestClamp(
            float offset,
            LateralClamp leftSideClamp,
            LateralClamp rightSideClamp)
        {
            float distanceToLeftSide = offset - leftSideClamp.Boundary;
            float distanceToRightSide = rightSideClamp.Boundary - offset;

            return distanceToLeftSide <= distanceToRightSide
                ? leftSideClamp
                : rightSideClamp;
        }

        private static float ClampOffset(float offset, LateralClamp clamp)
        {
            return clamp.Direction == LateralClampDirection.UpperBound
                ? Math.Min(offset, clamp.Boundary)
                : Math.Max(offset, clamp.Boundary);
        }

        private void MoveForward(float deltaTime, float finishCoordinate)
        {
            float coordinate = _model.CurrentRunnerCoordinate.Value + _settings.ForwardSpeed * deltaTime;

            _model.CurrentRunnerCoordinate.Value = Math.Min(coordinate, finishCoordinate);
        }

        private void CorrectLateralOffset(float deltaTime)
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

        private void UpdateLateralDirection()
        {
            float lateralOffset = _model.LateralOffset.Value;
            float lateralDelta = lateralOffset - _previousLateralOffset;

            _previousLateralOffset = lateralOffset;

            _model.LateralDirection.Value = lateralDelta switch
            {
                > LateralOffsetEpsilon => RunnerLateralDirection.Right,
                < -LateralOffsetEpsilon => RunnerLateralDirection.Left,
                _ => RunnerLateralDirection.None
            };
        }

        private void SetNormalizedLateralOffset(float normalizedOffset)
        {
            if (_correctingLaneBarrier != null)
            {
                return;
            }

            float clampedNormalizedOffset = Math.Clamp(normalizedOffset, NormalizedLateralOffsetMin, NormalizedLateralOffsetMax);
            float offset = clampedNormalizedOffset * _lateralHalfRange;

            foreach (LateralClamp clamp in _activeLaneBarrierClamps.Values)
            {
                offset = ClampOffset(offset, clamp);
            }

            _model.LateralOffset.Value = offset;
        }
    }
}

using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.Obstacle;
using Core.Gameplay.Track;
using Core.Gameplay.TransformRotator;
using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using ViewComponents.LaneBarriers;
using ViewComponents.Obstacles;
using ViewComponents.Track;
using ViewComponents.TransformRotators;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Level
{
    public sealed class CurrentLevel
        : IObstacleRegistry,
          ILaneBarrierRegistry,
          IWealthPointsModifierRegistry,
          ITransformRotatorsRegistry,
          ITrackProvider
    {
        private IReadOnlyCollection<IObstacle> _obstacles = Array.Empty<IObstacle>();
        private IReadOnlyCollection<ILaneBarrier> _barriers = Array.Empty<ILaneBarrier>();
        private IReadOnlyCollection<IWealthPointsModifier> _modifiers = Array.Empty<IWealthPointsModifier>();
        private IReadOnlyCollection<ITransformRotatorView> _rotators = Array.Empty<ITransformRotatorView>();
        private float _startCoordinate;
        private float _finishCoordinate;

        IReadOnlyCollection<IObstacle> IObstacleRegistry.Obstacles => _obstacles;
        IReadOnlyCollection<ILaneBarrier> ILaneBarrierRegistry.Barriers => _barriers;
        IReadOnlyCollection<IWealthPointsModifier> IWealthPointsModifierRegistry.Modifiers => _modifiers;
        IReadOnlyCollection<ITransformRotatorView> ITransformRotatorsRegistry.Rotators => _rotators;
        float ITrackProvider.StartCoordinate => _startCoordinate;
        float ITrackProvider.FinishCoordinate => _finishCoordinate;
        float ITrackProvider.RunLength => _finishCoordinate - _startCoordinate;
        internal TrackPath Track { get; private set; }

        internal void Set(Level level)
        {
            Obstacle[] obstacles = level.GetComponentsInChildren<Obstacle>();
            LaneBarrierZone[] barriers = level.GetComponentsInChildren<LaneBarrierZone>();
            WealthPointsModifierCollider[] modifiers = level.GetComponentsInChildren<WealthPointsModifierCollider>();
            TransformRotator[] rotators = level.GetComponentsInChildren<TransformRotator>();

            TrackPath track = new TrackPath(level.SplineContainer);

            Guard.AgainstNonPositive(track.Length, () => new InvalidSplineLengthException(level.gameObject.name, track.Length));

            float startCoordinate = track.NearestCoordinateTo(level.StartPosition);
            float finishCoordinate = track.NearestCoordinateTo(level.FinishPosition);

            Guard.AgainstNonPositive(
                finishCoordinate - startCoordinate,
                () => new InvalidLevelRunException(level.gameObject.name, startCoordinate, finishCoordinate)
            );

            foreach (LaneBarrierZone barrier in barriers)
            {
                barrier.Initialize(track);
            }

            Track = track;
            _startCoordinate = startCoordinate;
            _finishCoordinate = finishCoordinate;
            _obstacles = obstacles;
            _barriers = barriers;
            _modifiers = modifiers;
            _rotators = rotators;
        }
    }
}

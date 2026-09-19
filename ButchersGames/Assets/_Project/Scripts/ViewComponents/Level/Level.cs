using System.Collections.Generic;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using UnityEngine.Splines;
using ViewComponents.Finish;
using ViewComponents.LaneBarriers;
using ViewComponents.Obstacles;
using ViewComponents.Track;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Level
{
    public sealed class Level : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;

        public IReadOnlyCollection<Obstacle> Obstacles { get; private set; }
        public IReadOnlyCollection<LaneBarrierZone> Barriers { get; private set; }
        public IReadOnlyCollection<WealthPointsModifierCollider> Modifiers { get; private set; }
        public FinishCollider Finish { get; private set; }
        public TrackPath Track { get; private set; }

        private void Awake()
        {
            Obstacles = GetComponentsInChildren<Obstacle>();
            Barriers = GetComponentsInChildren<LaneBarrierZone>();
            Modifiers = GetComponentsInChildren<WealthPointsModifierCollider>();
            Finish = GetComponentInChildren<FinishCollider>();
            SplineContainer splineContainer = GetComponentInChildren<SplineContainer>();

            Guard.AgainstNull(Finish, () => new MissingFinishColliderException(gameObject.name));
            Guard.AgainstNull(splineContainer, () => new MissingSplineContainerException(gameObject.name));

            Track = new TrackPath(splineContainer);

            Guard.AgainstNonPositive(Track.Length, () => new InvalidSplineLengthException(gameObject.name, Track.Length));

            foreach (LaneBarrierZone barrier in Barriers)
            {
                barrier.Initialize(Track);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_playerSpawnPoint)
            {
                return;
            }

            Gizmos.color = Color.magenta;
            Matrix4x4 gizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = _playerSpawnPoint.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.up * 0.5f + Vector3.forward, 0.5f);
            Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
            Gizmos.matrix = gizmosMatrix;
        }
#endif
    }
}

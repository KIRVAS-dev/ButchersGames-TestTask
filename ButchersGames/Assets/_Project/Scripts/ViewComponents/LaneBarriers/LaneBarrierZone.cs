using System;
using Core.Gameplay.LaneBarrier;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using ViewComponents.Track;

namespace ViewComponents.LaneBarriers
{
    [RequireComponent(typeof(Collider))]
    public sealed class LaneBarrierZone
        : MonoBehaviour,
          ILaneBarrier
    {
        private float _minLateralOffset;
        private float _maxLateralOffset;

        public event Action<ILaneBarrier> Entered;
        public event Action<ILaneBarrier> Exited;

        float ILaneBarrier.MinLateralOffset => _minLateralOffset;
        float ILaneBarrier.MaxLateralOffset => _maxLateralOffset;

        public void Initialize(TrackPath track)
        {
            Collider zoneCollider = GetComponent<Collider>();

            Guard.AgainstTrue(!zoneCollider.isTrigger, () => new InvalidLaneBarrierColliderException(gameObject.name));

            (_minLateralOffset, _maxLateralOffset) = CalculateLateralRange(zoneCollider, track);
        }

        private void OnTriggerEnter(Collider other)
        {
            Entered?.Invoke(this);
        }

        private void OnTriggerExit(Collider other)
        {
            Exited?.Invoke(this);
        }

        private (float min, float max) CalculateLateralRange(Collider zoneCollider, TrackPath track)
        {
            Bounds bounds = zoneCollider.bounds;

            TrackPoint nearestPoint = track.NearestPointTo(bounds.center);
            Vector3 lateralAxis = Vector3.Cross(nearestPoint.Forward, Vector3.up).normalized;

            float centerProjection = Vector3.Dot(bounds.center - nearestPoint.Position, lateralAxis);

            float extentProjection = Mathf.Abs(bounds.extents.x * lateralAxis.x)
              + Mathf.Abs(bounds.extents.y * lateralAxis.y)
              + Mathf.Abs(bounds.extents.z * lateralAxis.z);

            return (centerProjection - extentProjection, centerProjection + extentProjection);
        }
    }
}

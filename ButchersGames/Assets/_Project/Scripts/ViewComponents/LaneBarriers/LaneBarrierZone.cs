using System;
using Core.Gameplay.LaneBarrier;
using Infrastructure.ExtendedExceptions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ViewComponents.LaneBarriers
{
    [RequireComponent(typeof(Collider))]
    public sealed class LaneBarrierZone
        : MonoBehaviour,
          ILaneBarrier
    {
        private Collider _collider;
        private float _minLateralOffset;
        private float _maxLateralOffset;

        public event Action<ILaneBarrier> Entered;
        public event Action<ILaneBarrier> Exited;

        float ILaneBarrier.MinLateralOffset => _minLateralOffset;
        float ILaneBarrier.MaxLateralOffset => _maxLateralOffset;

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            Validate();

            SplineContainer splineContainer = FindAnyObjectByType<SplineContainer>();
            Guard.AgainstNull(splineContainer, () => new MissingLaneBarrierSplineContainerException(gameObject.name));

            (_minLateralOffset, _maxLateralOffset) = CalculateLateralRange(splineContainer);
        }

        private void OnTriggerEnter(Collider other)
        {
            Entered?.Invoke(this);
        }

        private void OnTriggerExit(Collider other)
        {
            Exited?.Invoke(this);
        }

        private (float min, float max) CalculateLateralRange(SplineContainer splineContainer)
        {
            Vector3 lateralAxis = CalculateLateralAxis(splineContainer, out Vector3 nearestTrackPoint);

            Bounds bounds = _collider.bounds;
            float centerProjection = Vector3.Dot(bounds.center - nearestTrackPoint, lateralAxis);

            float extentProjection = Mathf.Abs(bounds.extents.x * lateralAxis.x)
              + Mathf.Abs(bounds.extents.y * lateralAxis.y)
              + Mathf.Abs(bounds.extents.z * lateralAxis.z);

            return (centerProjection - extentProjection, centerProjection + extentProjection);
        }

        private Vector3 CalculateLateralAxis(SplineContainer splineContainer, out Vector3 nearestTrackPoint)
        {
            float3 worldCenter = _collider.bounds.center;

            SplineUtility.GetNearestPoint(splineContainer.Spline, worldCenter, out _, out float t);
            splineContainer.Evaluate(t, out float3 position, out float3 tangent, out _);

            nearestTrackPoint = position;

            Vector3 forward = ((Vector3)tangent).normalized;
            return Vector3.Cross(forward, Vector3.up).normalized;
        }

        private void Validate()
        {
            Guard.AgainstTrue(!_collider.isTrigger, () => new InvalidLaneBarrierColliderException(gameObject.name));
        }
    }
}

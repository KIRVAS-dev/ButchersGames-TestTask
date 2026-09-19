using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ViewComponents.Track
{
    public sealed class TrackPath
    {
        private const float SplineStartParameter = 0f;
        private const float SplineEndParameter = 1f;
        private const int FirstKnotIndex = 0;
        private const int SecondKnotIndex = 1;
        private const int LastKnotIndexFromEnd = 1;
        private const int PreviousKnotIndexFromEnd = 2;

        private readonly SplineContainer _splineContainer;

        public TrackPath(SplineContainer splineContainer)
        {
            _splineContainer = splineContainer;

            Length = splineContainer.CalculateLength();
        }

        public float Length { get; }

        public TrackPoint PointAt(float distance)
        {
            Spline spline = _splineContainer.Spline;

            float distanceFraction = Mathf.Clamp01(distance / Length);

            float splineParameter = SplineUtility.GetNormalizedInterpolation(
                spline,
                distanceFraction * spline.GetLength(),
                PathIndexUnit.Distance
            );

            return PointAtSplineParameter(spline, splineParameter);
        }

        public TrackPoint NearestPointTo(Vector3 worldPoint)
        {
            Spline spline = _splineContainer.Spline;
            float3 localPoint = _splineContainer.transform.InverseTransformPoint(worldPoint);

            SplineUtility.GetNearestPoint(spline, localPoint, out _, out float splineParameter);

            return PointAtSplineParameter(spline, splineParameter);
        }

        private TrackPoint PointAtSplineParameter(Spline spline, float splineParameter)
        {
            _splineContainer.Evaluate(splineParameter, out float3 position, out float3 tangent, out _);

            Vector3 forward = ((Vector3)tangent).normalized;
            bool hasMovementDirection = forward.sqrMagnitude > Mathf.Epsilon;

            if (!hasMovementDirection)
            {
                forward = EndpointForward(spline, splineParameter);
            }

            return new TrackPoint(position, forward);
        }

        private Vector3 EndpointForward(Spline spline, float splineParameter)
        {
            bool isStart = splineParameter <= SplineStartParameter;
            bool isEnd = splineParameter >= SplineEndParameter;
            bool isEndpoint = isStart || isEnd;

            if (!isEndpoint)
            {
                throw new InvalidSplineTangentException(_splineContainer.name);
            }

            BezierKnot from = isStart
                ? spline[FirstKnotIndex]
                : spline[^PreviousKnotIndexFromEnd];

            BezierKnot to = isStart
                ? spline[SecondKnotIndex]
                : spline[^LastKnotIndexFromEnd];

            return _splineContainer.transform.TransformDirection(to.Position - from.Position).normalized;
        }
    }
}

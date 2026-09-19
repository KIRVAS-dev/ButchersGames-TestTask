using Core;
using UnityEngine;
using ViewComponents.Track;
using VContainer;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerMovementView
        : MonoBehaviour,
          IRunnerMovementView,
          IPresentationTickable
    {
        private TrackProvider _trackProvider;
        private float _distance;
        private float _lateralOffset;
        private bool _isTransformDirty;

        [Inject]
        private void Construct(TrackProvider trackProvider)
        {
            _trackProvider = trackProvider;
        }

        void IPresentationTickable.Tick()
        {
            if (!_isTransformDirty)
            {
                return;
            }

            _isTransformDirty = false;

            TrackPoint point = _trackProvider.PointAt(_distance);

            Vector3 lateralAxis = Vector3.Cross(point.Forward, Vector3.up).normalized;
            Vector3 position = point.Position + lateralAxis * _lateralOffset;
            Quaternion rotation = Quaternion.LookRotation(point.Forward, Vector3.up);

            transform.SetPositionAndRotation(position, rotation);
        }

        public void SetDistance(float distance)
        {
            _distance = distance;
            _isTransformDirty = true;
        }

        public void SetLateralOffset(float lateralOffset)
        {
            _lateralOffset = lateralOffset;
            _isTransformDirty = true;
        }
    }
}

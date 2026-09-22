using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using Unity.Cinemachine;
using UnityEngine;

namespace ViewComponents.CelebrationCamera
{
    [DisallowMultipleComponent]
    public sealed class CelebrationCameraView
        : MonoBehaviour,
          ICelebrationCameraView
    {
        private const float LeftArcSign = -1f;
        private const float MidpointBias = 0.5f;
        private const float HalfDurationRatio = 0.5f;
        private const int InfiniteLoopCount = -1;

        [SerializeField] private CelebrationCameraConfig _config;
        [SerializeField] private CinemachineFollow _cinemachineFollow;
        [SerializeField] private Transform _orbitPivot;

        private Tween _motion;
        private Vector3 _restPosition;
        private Quaternion _restRotation;
        private float _radius;
        private float _arc;

        private void Awake()
        {
            Validate();
            RememberRestPose();
        }

        private void OnDestroy()
        {
            KillMotion();
        }

        void ICelebrationCameraView.Play()
        {
            KillMotion();
            RememberRestPose();
            _arc = 0f;
            DisableFollow();

            float leftArc = LeftArcSign * _config.ArcDistance;
            Sequence intro = CreateLeg(0f, leftArc);
            intro.OnComplete(BeginLeftRight);
            intro.SetLink(_cinemachineFollow.gameObject);
            _motion = intro;
            intro.Play();
        }

        void ICelebrationCameraView.Stop()
        {
            float arc = _arc;

            KillMotion();

            if (Mathf.Approximately(arc, 0f))
            {
                EnableFollow();

                return;
            }

            DisableFollow();

            Sequence returnLeg = CreateLeg(arc, 0f);
            returnLeg.OnComplete(FinishReturn);
            returnLeg.SetLink(_cinemachineFollow.gameObject);
            _motion = returnLeg;
            returnLeg.Play();
        }

        void ICelebrationCameraView.Cancel()
        {
            KillMotion();
            _cinemachineFollow.transform.SetPositionAndRotation(_restPosition, _restRotation);
            _arc = 0f;
            EnableFollow();
        }

        private void RememberRestPose()
        {
            _restPosition = _cinemachineFollow.transform.position;
            _restRotation = _cinemachineFollow.transform.rotation;
            _radius = HorizontalDistance();
        }

        private void BeginLeftRight()
        {
            float leftArc = LeftArcSign * _config.ArcDistance;
            float rightArc = _config.ArcDistance;

            Sequence cycle = DOTween.Sequence();
            cycle.Pause();
            cycle.Append(CreateLeg(leftArc, rightArc));
            cycle.Append(CreateLeg(rightArc, leftArc));
            cycle.SetLoops(InfiniteLoopCount);
            cycle.SetLink(_cinemachineFollow.gameObject);
            _motion = cycle;
            cycle.Play();
        }

        private void FinishReturn()
        {
            _motion = null;
            EnableFollow();
        }

        private Sequence CreateLeg(float fromArc, float toArc)
        {
            float midpoint = Mathf.Lerp(fromArc, toArc, MidpointBias);
            float halfDuration = Mathf.Abs(toArc - fromArc) / _config.ArcSpeed * HalfDurationRatio;

            Sequence leg = DOTween.Sequence();
            leg.Pause();
            leg.Append(DOTween.To(() => _arc, ApplyArc, midpoint, halfDuration).SetEase(_config.AccelerationEase));
            leg.Append(DOTween.To(() => _arc, ApplyArc, toArc, halfDuration).SetEase(_config.DecelerationEase));

            return leg;
        }

        private void ApplyArc(float arc)
        {
            float deltaDegrees = (arc - _arc) / _radius * Mathf.Rad2Deg;
            _arc = arc;
            _cinemachineFollow.transform.RotateAround(_orbitPivot.position, Vector3.up, deltaDegrees);
        }

        private float HorizontalDistance()
        {
            Vector3 offset = _cinemachineFollow.transform.position - _orbitPivot.position;
            offset.y = 0f;

            return offset.magnitude;
        }

        private void EnableFollow() => _cinemachineFollow.enabled = true;

        private void DisableFollow() => _cinemachineFollow.enabled = false;

        private void KillMotion()
        {
            _motion?.Kill();
            _motion = null;
        }

        private void Validate()
        {
            Guard.AgainstNull(_config, () => new MissingCelebrationCameraConfigException(nameof(_config), gameObject.name));
            Guard.AgainstNull(_cinemachineFollow, () => new MissingCelebrationCameraFollowException(nameof(_cinemachineFollow), gameObject.name));
            Guard.AgainstNull(_orbitPivot, () => new MissingCelebrationCameraTargetException(nameof(_orbitPivot), gameObject.name));

            _config.Validate();

            float radius = HorizontalDistance();

            Guard.AgainstNonPositive(radius, () => new InvalidCelebrationCameraRadiusException(gameObject.name, radius));

            Guard.AgainstTrue(
                _config.ArcDistance >= Mathf.PI * radius,
                () => new CelebrationCameraArcTooLongException(_config.ArcDistance, radius)
            );
        }
    }
}

using DG.Tweening;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using Unity.Cinemachine;
using UnityEngine;

namespace ViewComponents.CelebrationCamera
{
    [DisallowMultipleComponent]
    public sealed class CelebrationCameraView
        : MonoBehaviour,
          ICelebrationCameraView,
          IValidatable
    {
        private const float LeftArcSign = -1f;
        private const float MidpointBias = 0.5f;
        private const float HalfDurationRatio = 0.5f;
        private const int InfiniteLoopCount = -1;

        [SerializeField] private CelebrationCameraConfig _config;
        [SerializeField] private CinemachineFollow _cinemachineFollow;
        [SerializeField] private Transform _orbitPivot;

        private Tween _motion;
        private float _radius;
        private float _arc;

        private void OnDestroy()
        {
            KillMotion();
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_config, () => new MissingCelebrationCameraConfigException(nameof(_config), gameObject.name));

            Guard.AgainstNull(
                _cinemachineFollow,
                () => new MissingCelebrationCameraFollowException(nameof(_cinemachineFollow), gameObject.name)
            );

            Guard.AgainstNull(
                _orbitPivot,
                () => new MissingCelebrationCameraTargetException(nameof(_orbitPivot), gameObject.name)
            );

            _config.Validate();

            float radius = HorizontalDistance();

            Guard.AgainstNonPositive(radius, () => new InvalidCelebrationCameraRadiusException(gameObject.name, radius));

            Guard.AgainstTrue(
                _config.ArcDistance >= Mathf.PI * radius,
                () => new CelebrationCameraArcTooLongException(_config.ArcDistance, radius)
            );
        }

        void ICelebrationCameraView.Play()
        {
            ResetMotion();
            CacheOrbitRadius();
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
            ResetMotion();
            EnableFollow();
            ResetFollowDamping();
        }

        private void CacheOrbitRadius() => _radius = HorizontalDistance();

        private void EnableFollow() => _cinemachineFollow.enabled = true;

        private void DisableFollow() => _cinemachineFollow.enabled = false;

        private void ResetFollowDamping() => _cinemachineFollow.VirtualCamera.PreviousStateIsValid = false;

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

        private void ResetMotion()
        {
            KillMotion();
            _arc = 0f;
        }

        private void KillMotion()
        {
            _motion?.Kill();
            _motion = null;
        }
    }
}

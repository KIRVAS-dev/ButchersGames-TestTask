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
        [SerializeField] private CinemachineFollow _follow;
        [SerializeField] private Transform _target;

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
            intro.SetLink(_follow.gameObject);
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
            returnLeg.SetLink(_follow.gameObject);
            _motion = returnLeg;
            returnLeg.Play();
        }

        void ICelebrationCameraView.Cancel()
        {
            KillMotion();
            _follow.transform.SetPositionAndRotation(_restPosition, _restRotation);
            _arc = 0f;
            EnableFollow();
        }

        private void RememberRestPose()
        {
            _restPosition = _follow.transform.position;
            _restRotation = _follow.transform.rotation;
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
            cycle.SetLink(_follow.gameObject);
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
            _follow.transform.RotateAround(_target.position, Vector3.up, deltaDegrees);
        }

        private float HorizontalDistance()
        {
            Vector3 offset = _follow.transform.position - _target.position;
            offset.y = 0f;

            return offset.magnitude;
        }

        private void EnableFollow() => _follow.enabled = true;

        private void DisableFollow() => _follow.enabled = false;

        private void KillMotion()
        {
            _motion?.Kill();
            _motion = null;
        }

        private void Validate()
        {
            Guard.AgainstNull(_config, () => new MissingCelebrationCameraConfigException(nameof(_config), gameObject.name));
            Guard.AgainstNull(_follow, () => new MissingCelebrationCameraFollowException(nameof(_follow), gameObject.name));
            Guard.AgainstNull(_target, () => new MissingCelebrationCameraTargetException(nameof(_target), gameObject.name));

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

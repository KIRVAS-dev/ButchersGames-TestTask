using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace UI.FloatingText
{
    [CreateAssetMenu(menuName = "Configs/Floating Text Config")]
    public sealed class FloatingTextConfig : ScriptableObject
    {
        [SerializeField] private float _sideOffset = 120f;
        [SerializeField] private int _prewarmCount = 8;

        [Header("Move")]
        [SerializeField] private float _lifetime = 0.8f;
        [SerializeField] private float _riseDistance = 150f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;

        [Header("Appear")]
        [SerializeField] private float _appearDuration = 0.15f;
        [SerializeField] private float _appearStartScale = 0.5f;
        [SerializeField] private Ease _appearEase = Ease.OutBack;

        [Header("Disappear")]
        [SerializeField] private float _disappearDuration = 0.3f;
        [SerializeField] private Ease _disappearEase = Ease.InQuad;

        public float SideOffset => _sideOffset;
        public int PrewarmCount => _prewarmCount;

        public float Lifetime => _lifetime;
        public float RiseDistance => _riseDistance;
        public Ease MoveEase => _moveEase;

        public float AppearDuration => _appearDuration;
        public float AppearStartScale => _appearStartScale;
        public Ease AppearEase => _appearEase;

        public float DisappearDuration => _disappearDuration;
        public Ease DisappearEase => _disappearEase;

        public void Validate()
        {
            Guard.AgainstNegative(_sideOffset, () => Invalid(nameof(_sideOffset), _sideOffset));
            Guard.AgainstNegative(_prewarmCount, () => Invalid(nameof(_prewarmCount), _prewarmCount));
            Guard.AgainstNonPositive(_lifetime, () => Invalid(nameof(_lifetime), _lifetime));
            Guard.AgainstNonPositive(_riseDistance, () => Invalid(nameof(_riseDistance), _riseDistance));
            Guard.AgainstNegative(_appearDuration, () => Invalid(nameof(_appearDuration), _appearDuration));
            Guard.AgainstNegative(_appearStartScale, () => Invalid(nameof(_appearStartScale), _appearStartScale));
            Guard.AgainstNegative(_disappearDuration, () => Invalid(nameof(_disappearDuration), _disappearDuration));

            Guard.AgainstGreaterThan(
                _appearDuration + _disappearDuration,
                _lifetime,
                () => new InvalidFloatingTextTimingException(_lifetime, _appearDuration, _disappearDuration)
            );

            return;

            ExtendedException Invalid(string fieldName, float value) => new InvalidFloatingTextValueException(fieldName, value);
        }
    }
}

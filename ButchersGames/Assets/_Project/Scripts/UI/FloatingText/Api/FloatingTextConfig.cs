using System;
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
            Func<string, float, ExtendedException> invalid =
                (fieldName, value) => new InvalidFloatingTextValueException(fieldName, value);

            Guard.AgainstNegative(_sideOffset, () => invalid(nameof(_sideOffset), _sideOffset));
            Guard.AgainstNegative(_prewarmCount, () => invalid(nameof(_prewarmCount), _prewarmCount));
            Guard.AgainstNonPositive(_lifetime, () => invalid(nameof(_lifetime), _lifetime));
            Guard.AgainstNonPositive(_riseDistance, () => invalid(nameof(_riseDistance), _riseDistance));
            Guard.AgainstNegative(_appearDuration, () => invalid(nameof(_appearDuration), _appearDuration));
            Guard.AgainstNegative(_appearStartScale, () => invalid(nameof(_appearStartScale), _appearStartScale));
            Guard.AgainstNegative(_disappearDuration, () => invalid(nameof(_disappearDuration), _disappearDuration));

            Guard.AgainstGreaterThan(
                _appearDuration + _disappearDuration,
                _lifetime,
                () => new InvalidFloatingTextTimingException(_lifetime, _appearDuration, _disappearDuration)
            );
        }
    }
}

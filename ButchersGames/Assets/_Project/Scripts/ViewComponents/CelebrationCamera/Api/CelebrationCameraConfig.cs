using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace ViewComponents.CelebrationCamera
{
    [CreateAssetMenu(menuName = "Configs/Celebration Camera Config")]
    internal sealed class CelebrationCameraConfig : ScriptableObject, IValidatable
    {
        [SerializeField] [Min(0f)]
        private float _arcDistance = 1.5f;

        [SerializeField] [Min(0f)]
        private float _arcSpeed = 1.5f;

        [SerializeField] private Ease _accelerationEase = Ease.InQuad;
        [SerializeField] private Ease _decelerationEase = Ease.OutQuad;

        internal float ArcDistance => _arcDistance;
        internal float ArcSpeed => _arcSpeed;
        internal Ease AccelerationEase => _accelerationEase;
        internal Ease DecelerationEase => _decelerationEase;

        public void Validate()
        {
            Guard.AgainstNonPositive(_arcDistance, () => Invalid(nameof(_arcDistance), _arcDistance));
            Guard.AgainstNonPositive(_arcSpeed, () => Invalid(nameof(_arcSpeed), _arcSpeed));

            return;

            ExtendedException Invalid(string fieldName, float value) =>
                new InvalidCelebrationCameraValueException(fieldName, value);
        }
    }
}

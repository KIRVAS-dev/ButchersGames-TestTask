using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace ViewComponents.Obstacles
{
    [CreateAssetMenu(menuName = "Configs/Obstacle Config")]
    internal sealed class ObstacleConfig : ScriptableObject, IValidatable
    {
        [SerializeField] [Min(0f)] private float _stopDuration;

        public float StopDuration => _stopDuration;

        public void Validate()
        {
            Guard.AgainstNegative(_stopDuration, () => new InvalidObstacleValueException(nameof(_stopDuration), _stopDuration));
        }
    }
}

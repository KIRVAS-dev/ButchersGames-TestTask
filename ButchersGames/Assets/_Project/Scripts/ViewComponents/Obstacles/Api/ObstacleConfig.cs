using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Obstacles
{
    [CreateAssetMenu(menuName = "Configs/Obstacle")]
    public sealed class ObstacleConfig : ScriptableObject
    {
        [SerializeField] [Min(0f)] private float _stopDuration;

        public float StopDuration => _stopDuration;

        public void Validate()
        {
            Guard.AgainstNegative(_stopDuration, () => new InvalidObstacleValueException(nameof(_stopDuration), _stopDuration));
        }
    }
}

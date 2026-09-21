using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.CharacterTurn
{
    [CreateAssetMenu(menuName = "Configs/Character Turn")]
    internal sealed class CharacterTurnConfig : ScriptableObject
    {
        [SerializeField] [Min(0f)] private float _maxAngle = 35f;
        [SerializeField] [Min(0f)] private float _turnSpeed = 200f;
        [SerializeField] [Min(0f)] private float _releaseDelay = 0.1f;

        public float MaxAngle => _maxAngle;
        public float TurnSpeed => _turnSpeed;
        public float ReleaseDelay => _releaseDelay;

        public void Validate()
        {
            Guard.AgainstNonPositive(_maxAngle, () => new InvalidCharacterTurnValueException(nameof(_maxAngle), _maxAngle));
            Guard.AgainstNonPositive(_turnSpeed, () => new InvalidCharacterTurnValueException(nameof(_turnSpeed), _turnSpeed));
            Guard.AgainstNegative(_releaseDelay, () => new InvalidCharacterTurnValueException(nameof(_releaseDelay), _releaseDelay));
        }
    }
}

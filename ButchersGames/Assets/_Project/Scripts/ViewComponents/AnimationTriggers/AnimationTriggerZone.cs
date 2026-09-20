using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.AnimationTriggers
{
    public sealed class AnimationTriggerZone : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animationName;

        private bool _isTriggered;

        private void Awake()
        {
            Validate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered)
            {
                return;
            }

            _isTriggered = true;

            _animator.Play(_animationName);
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _collider,
                () => new MissingAnimationTriggerZoneFieldException(nameof(_collider), gameObject.name)
            );

            Guard.AgainstNull(_animator, () => new MissingAnimationTriggerZoneFieldException(nameof(_animator), gameObject.name));

            Guard.AgainstTrue(
                string.IsNullOrEmpty(_animationName),
                () => new MissingAnimationTriggerZoneFieldException(nameof(_animationName), gameObject.name)
            );

            Guard.AgainstTrue(!_collider.isTrigger, () => new InvalidAnimationTriggerZoneColliderException(gameObject.name));
        }
    }
}

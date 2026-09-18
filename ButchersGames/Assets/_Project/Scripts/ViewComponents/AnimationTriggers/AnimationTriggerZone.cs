using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.AnimationTriggers
{
    [RequireComponent(typeof(Collider))]
    public sealed class AnimationTriggerZone : MonoBehaviour
    {
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
            Guard.AgainstNull(_animator, () => new MissingAnimationTriggerZoneFieldException(nameof(_animator), gameObject.name));

            Guard.AgainstTrue(
                string.IsNullOrEmpty(_animationName),
                () => new MissingAnimationTriggerZoneFieldException(nameof(_animationName), gameObject.name)
            );

            Collider zoneCollider = GetComponent<Collider>();
            Guard.AgainstTrue(!zoneCollider.isTrigger, () => new InvalidAnimationTriggerZoneColliderException(gameObject.name));
        }
    }
}

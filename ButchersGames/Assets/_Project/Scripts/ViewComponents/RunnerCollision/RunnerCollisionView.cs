using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.RunnerCollision
{
    [DisallowMultipleComponent]
    public sealed class RunnerCollisionView
        : MonoBehaviour,
          IRunnerCollisionView,
          IValidatable
    {
        [SerializeField] private Rigidbody _rigidbody;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_rigidbody, () => new MissingRunnerCollisionFieldException(nameof(_rigidbody), gameObject.name));
        }

        void IRunnerCollisionView.EnableCollisions()
        {
            _rigidbody.detectCollisions = true;
        }

        void IRunnerCollisionView.DisableCollisions()
        {
            _rigidbody.detectCollisions = false;
        }
    }
}

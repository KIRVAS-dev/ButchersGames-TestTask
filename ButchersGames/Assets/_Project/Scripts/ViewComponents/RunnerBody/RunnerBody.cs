using ContentValidation;
using Core.Gameplay.RunnerBody;
using Core.Gameplay.RunnerCollision;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.RunnerBody
{
    [DisallowMultipleComponent]
    public sealed class RunnerBody
        : MonoBehaviour,
          IRunnerBodyProvider,
          IRunnerCollision,
          IValidatable
    {
        private const float DiameterFactor = 2f;

        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Rigidbody _rigidbody;

        float IRunnerBodyProvider.Width => CalculateWidth();

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_collider, () => Missing(nameof(_collider)));
            Guard.AgainstNull(_rigidbody, () => Missing(nameof(_rigidbody)));

            float width = CalculateWidth();

            Guard.AgainstNonPositive(width, () => new InvalidRunnerBodyValueException(nameof(_collider), gameObject.name, width));

            return;

            ExtendedException Missing(string fieldName) => new MissingRunnerBodyFieldException(fieldName, gameObject.name);
        }

        void IRunnerCollision.EnableCollisions()
        {
            _rigidbody.detectCollisions = true;
        }

        void IRunnerCollision.DisableCollisions()
        {
            _rigidbody.detectCollisions = false;
        }

        private float CalculateWidth()
        {
            Vector3 scale = _collider.transform.lossyScale;
            float horizontalScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));

            return _collider.radius * DiameterFactor * horizontalScale;
        }
    }
}

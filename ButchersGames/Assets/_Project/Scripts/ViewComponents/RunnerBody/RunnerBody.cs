using ContentValidation;
using Core.Gameplay.RunnerBody;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.RunnerBody
{
    [DisallowMultipleComponent]
    public sealed class RunnerBody
        : MonoBehaviour,
          IRunnerBody,
          IValidatable
    {
        private const float DiameterFactor = 2f;

        [SerializeField] private CapsuleCollider _collider;

        float IRunnerBody.Width => CalculateWidth();

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_collider, () => new MissingRunnerBodyFieldException(nameof(_collider), gameObject.name));

            float width = CalculateWidth();

            Guard.AgainstNonPositive(width, () => new InvalidRunnerBodyValueException(nameof(_collider), gameObject.name, width));
        }

        private float CalculateWidth()
        {
            Vector3 scale = _collider.transform.lossyScale;
            float horizontalScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));

            return _collider.radius * DiameterFactor * horizontalScale;
        }
    }
}

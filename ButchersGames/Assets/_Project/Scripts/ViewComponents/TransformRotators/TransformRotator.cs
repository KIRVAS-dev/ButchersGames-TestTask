using ContentValidation;
using Core.Gameplay.TransformRotator;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TransformRotators
{
    internal sealed class TransformRotator
        : MonoBehaviour,
          ITransformRotatorView,
          IValidatable
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Vector3 _speed;
        [SerializeField] private RotationDirection _direction = RotationDirection.Clockwise;

        private float DirectionSign => _direction == RotationDirection.Clockwise
            ? -1f
            : 1f;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(
                _targetTransform,
                () => new MissingTransformRotatorFieldException(nameof(_targetTransform), gameObject.name)
            );
        }

        void ITransformRotatorView.Rotate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            _targetTransform.Rotate(_speed * DirectionSign * Time.deltaTime);
        }
    }
}

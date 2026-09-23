using ContentValidation;
using Core.Lifecycle;
using Core.Loop;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.CharacterTurn
{
    [DisallowMultipleComponent]
    public sealed class CharacterTurnView
        : MonoBehaviour,
          ICharacterTurnView,
          IPresentationTickable,
          IValidatable,
          IWarmupLifecycle
    {
        [SerializeField] private CharacterTurnConfig _config;

        private TurnAnimator _turnAnimator;
        private Quaternion _baseLocalRotation;
        private float _appliedAngle;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_config, () => new MissingCharacterTurnConfigException(nameof(_config), gameObject.name));

            _config.Validate();
        }

        void IWarmupLifecycle.Warmup()
        {
            _turnAnimator = new TurnAnimator(_config);
            _baseLocalRotation = transform.localRotation;
        }

        void IPresentationTickable.Tick()
        {
            _turnAnimator.Tick(Time.deltaTime);

            if (Mathf.Approximately(_turnAnimator.CurrentAngle, _appliedAngle))
            {
                return;
            }

            _appliedAngle = _turnAnimator.CurrentAngle;

            transform.localRotation = Quaternion.AngleAxis(_appliedAngle, Vector3.up) * _baseLocalRotation;
        }

        void ICharacterTurnView.SetTurn(CharacterTurnSide side)
        {
            _turnAnimator.SetSide(side);
        }
    }
}

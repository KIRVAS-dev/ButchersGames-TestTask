using Core.Loop;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.CharacterTurn
{
    [DisallowMultipleComponent]
    public sealed class CharacterTurnView
        : MonoBehaviour,
          ICharacterTurnView,
          IPresentationTickable
    {
        [SerializeField] private CharacterTurnConfig _config;

        private TurnAnimator _turnAnimator;
        private Quaternion _baseLocalRotation;
        private float _appliedAngle;

        private void Awake()
        {
            Validate();

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

        public void SetTurn(CharacterTurnSide side)
        {
            _turnAnimator.SetSide(side);
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _config,
                () => new MissingCharacterTurnConfigException(nameof(_config), gameObject.name)
            );

            _config.Validate();
        }
    }
}

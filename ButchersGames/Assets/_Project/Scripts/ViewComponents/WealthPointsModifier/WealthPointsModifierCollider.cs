using System;
using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthPointsModifier
{
    public sealed class WealthPointsModifierCollider
        : MonoBehaviour,
          IWealthPointsModifier
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private WealthPointsModifierConfig _config;

        private bool _isTriggered;

        public event Action<WealthPointsModifierType, int> Triggered;

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

            Triggered?.Invoke(_config.Type, _config.Amount);

            if (TryGetComponent(out ITriggerReaction reaction))
            {
                reaction.React();
            }
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _collider,
                () => new MissingWealthPointsModifierConfigException(nameof(_collider), gameObject.name)
            );

            Guard.AgainstNull(_config, () => new MissingWealthPointsModifierConfigException(nameof(_config), gameObject.name));
            Guard.AgainstTrue(!_collider.isTrigger, () => new InvalidWealthPointsModifierColliderException(gameObject.name));

            _config.Validate();
        }
    }
}

using System;
using Core.Gameplay.WealthPointsModifier;
using ExtendedExceptions;
using UnityEngine;
using ViewComponents.Collectables;

namespace ViewComponents.WealthPointsModifier
{
    [RequireComponent(typeof(Collider))]
    public sealed class WealthPointsModifierCollider
        : MonoBehaviour,
          IWealthPointsModifier
    {
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

            if (TryGetComponent(out Collectable collectable))
            {
                collectable.Collect();
            }
        }

        private void Validate()
        {
            Guard.AgainstNull(_config, () => new MissingWealthPointsModifierConfigException(nameof(_config), gameObject.name));

            _config.Validate();
        }
    }
}

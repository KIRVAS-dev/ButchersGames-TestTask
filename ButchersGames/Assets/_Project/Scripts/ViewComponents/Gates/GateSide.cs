using System;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Gates
{
    internal sealed class GateSide
        : MonoBehaviour,
          IValidatable
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private WealthPointsModifierConfig _config;

        private Action<GateSide> _entered;

        internal WealthPointsModifierConfig Config => _config;

        internal void Initialize(Action<GateSide> entered)
        {
            _entered = entered;
        }

        private void OnTriggerEnter(Collider other)
        {
            _entered.Invoke(this);
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_collider, () => Missing(nameof(_collider)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));
            Guard.AgainstTrue(!_collider.isTrigger, () => new InvalidGateSideColliderException(gameObject.name));

            _config.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingGateFieldException(fieldName, gameObject.name);
        }
    }
}

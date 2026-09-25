using System;
using ContentValidation;
using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Gates
{
    internal sealed class Gate
        : MonoBehaviour,
          IWealthPointsModifier,
          IValidatable
    {
        private const int MinSideCount = 2;

        [SerializeField] private GateSide[] _sides;

        private bool _isTriggered;

        public event Action<WealthPointsModifierType, int> Triggered;

        void IValidatable.Validate()
        {
            Guard.AgainstNullOrEmpty(_sides, () => new MissingGateFieldException(nameof(_sides), gameObject.name));

            Guard.AgainstLessThan(
                _sides.Length,
                MinSideCount,
                () => new InvalidGateValueException(nameof(_sides), gameObject.name, _sides.Length)
            );

            foreach (GateSide side in _sides)
            {
                Guard.AgainstNull(side, () => new MissingGateFieldException(nameof(_sides), gameObject.name));
            }

            foreach (GateSide childSide in GetComponentsInChildren<GateSide>(true))
            {
                Guard.AgainstTrue(
                    Array.IndexOf(_sides, childSide) < 0,
                    () => new UnlistedGateSideException(childSide.gameObject.name, gameObject.name)
                );
            }
        }

        internal void Initialize()
        {
            foreach (GateSide side in _sides)
            {
                side.Initialize(Enter);
            }
        }

        private void Enter(GateSide side)
        {
            if (_isTriggered)
            {
                return;
            }

            _isTriggered = true;

            Triggered?.Invoke(side.Config.ModifierType, side.Config.WealthPoints);

            gameObject.SetActive(false);
        }
    }
}

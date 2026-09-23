using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace ViewComponents.WealthPointsModifier
{
    [CreateAssetMenu(menuName = "Configs/Wealth Points Modifier Config")]
    public sealed class WealthPointsModifierConfig : ScriptableObject, IValidatable
    {
        [SerializeField] private WealthPointsModifierType _modifierType;
        [SerializeField] [Min(1)] private int _wealthPoints;

        internal WealthPointsModifierType ModifierType => _modifierType;
        internal int WealthPoints => _wealthPoints;

        public void Validate()
        {
            Guard.AgainstNonPositive(_wealthPoints, () => new InvalidWealthPointsModifierValueException(nameof(_wealthPoints), _wealthPoints));
        }
    }
}

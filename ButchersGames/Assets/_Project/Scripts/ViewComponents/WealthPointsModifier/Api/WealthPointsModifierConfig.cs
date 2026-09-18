using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthPointsModifier
{
    [CreateAssetMenu(menuName = "Configs/Wealth Points Modifier")]
    public sealed class WealthPointsModifierConfig : ScriptableObject
    {
        [SerializeField] private WealthPointsModifierType _type;
        [SerializeField] [Min(1)] private int _amount;

        public WealthPointsModifierType Type => _type;
        public int Amount => _amount;

        public void Validate()
        {
            Guard.AgainstNonPositive(_amount, () => new InvalidWealthPointsModifierValueException(nameof(_amount), _amount));
        }
    }
}

using ExtendedExceptions;
using UnityEngine;

namespace Core.Gameplay.WealthPointsModifier
{
    [CreateAssetMenu(menuName = "Data/Wealth Points Modifier")]
    public sealed class WealthPointsModifierConfig : ScriptableObject
    {
        [SerializeField] private WealthPointsModifierType _type;
        [SerializeField] [Min(1)] private int _amount;
        [SerializeField] private bool _deactivateGameObjectOnTrigger;

        public WealthPointsModifierType Type => _type;
        public int Amount => _amount;
        public bool DeactivateGameObjectOnTrigger => _deactivateGameObjectOnTrigger;

        public void Validate()
        {
            Guard.AgainstNonPositive(_amount, () => new InvalidWealthPointsModifierValueException(nameof(_amount), _amount));
        }
    }
}

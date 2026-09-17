using System;

namespace Core.Gameplay.WealthPointsModifier
{
    public interface IWealthPointsModifier
    {
        event Action<WealthPointsModifierType, int> Triggered;
    }
}

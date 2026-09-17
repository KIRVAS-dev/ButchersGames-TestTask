using System;
using System.Collections.Generic;

namespace Core.Gameplay.WealthPointsModifier
{
    public interface IWealthPointsModifierRegistry
    {
        event Action ModifiersChanged;

        IReadOnlyList<IWealthPointsModifier> Modifiers { get; }
    }
}

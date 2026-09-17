using System;
using System.Collections.Generic;

namespace Core.Gameplay.WealthPointsModifier
{
    public interface IWealthPointsModifierRegistry
    {
        IReadOnlyList<IWealthPointsModifier> Modifiers { get; }

        event Action ModifiersChanged;
    }
}

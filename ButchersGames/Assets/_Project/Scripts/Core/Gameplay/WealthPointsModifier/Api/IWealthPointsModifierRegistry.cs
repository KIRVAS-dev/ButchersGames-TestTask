using System.Collections.Generic;

namespace Core.Gameplay.WealthPointsModifier
{
    public interface IWealthPointsModifierRegistry
    {
        IReadOnlyCollection<IWealthPointsModifier> Modifiers { get; }
    }
}

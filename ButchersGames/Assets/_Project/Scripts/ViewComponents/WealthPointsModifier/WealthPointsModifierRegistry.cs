using System;
using System.Collections.Generic;
using Core.Gameplay.WealthPointsModifier;

namespace ViewComponents.WealthPointsModifier
{
    public sealed class WealthPointsModifierRegistry
        : SceneRegistry<WealthPointsModifierCollider, IWealthPointsModifier>,
          IWealthPointsModifierRegistry
    {
        public event Action ModifiersChanged;

        public IReadOnlyList<IWealthPointsModifier> Modifiers => Items;

        protected override void NotifyItemsChanged()
        {
            ModifiersChanged?.Invoke();
        }
    }
}

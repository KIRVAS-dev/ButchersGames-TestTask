using System.Collections.Generic;
using Core.Gameplay.WealthPointsModifier;
using ViewComponents.Level;

namespace ViewComponents.WealthPointsModifier
{
    public sealed class WealthPointsModifierRegistry : IWealthPointsModifierRegistry
    {
        private readonly LevelProvider _levelProvider;

        public WealthPointsModifierRegistry(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;
        }

        public IReadOnlyCollection<IWealthPointsModifier> Modifiers => _levelProvider.Modifiers;
    }
}

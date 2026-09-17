using System;
using System.Collections.Generic;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthPointsModifier;
using UnityEngine;
using VContainer;

namespace ViewComponents.WealthPointsModifier
{
    public sealed class WealthPointsModifierRegistry
        : MonoBehaviour,
          IWealthPointsModifierRegistry
    {
        private ILevelProvider _levelProvider;
        private List<IWealthPointsModifier> _modifiers = new List<IWealthPointsModifier>();

        public event Action ModifiersChanged;

        public IReadOnlyList<IWealthPointsModifier> Modifiers => _modifiers;

        [Inject]
        private void Construct(ILevelProvider levelProvider)
        {
            _levelProvider = levelProvider;

            _levelProvider.LevelLoaded += Rescan;
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= Rescan;
        }

        private void Rescan()
        {
            _modifiers = new List<IWealthPointsModifier>(
                FindObjectsByType<WealthPointsModifierCollider>(FindObjectsInactive.Exclude)
            );

            ModifiersChanged?.Invoke();
        }
    }
}

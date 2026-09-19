using System.Collections.Generic;
using Core.Gameplay.LevelProgression;
using UnityEngine;
using VContainer;

namespace ViewComponents
{
    public abstract class SceneRegistry<TComponent, TItem> : MonoBehaviour where TComponent : Component, TItem
    {
        private ILevelProvider _levelProvider;
        private List<TItem> _items = new List<TItem>();

        protected IReadOnlyList<TItem> Items => _items;

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

        protected abstract void NotifyItemsChanged();

        private void Rescan()
        {
            _items = new List<TItem>(FindObjectsByType<TComponent>(FindObjectsInactive.Exclude));

            NotifyItemsChanged();
        }
    }
}

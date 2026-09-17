using System;
using Core.Gameplay.Finish;
using Core.Gameplay.LevelProgression;
using ExtendedExceptions;
using UnityEngine;
using VContainer;

namespace ViewComponents.Finish
{
    public sealed class FinishProvider
        : MonoBehaviour,
          IFinishProvider
    {
        private ILevelProvider _levelProvider;
        private FinishCollider _current;

        public event Action Reached;

        [Inject]
        private void Construct(ILevelProvider levelProvider)
        {
            _levelProvider = levelProvider;

            _levelProvider.LevelLoaded += Rescan;
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= Rescan;
            UnsubscribeCurrent();
        }

        private void Rescan()
        {
            UnsubscribeCurrent();

            _current = FindAnyObjectByType<FinishCollider>();

            Guard.AgainstNull(_current, () => new MissingFinishColliderException(gameObject.name));

            _current.Reached += OnCurrentReached;
        }

        private void UnsubscribeCurrent()
        {
            if (_current != null)
            {
                _current.Reached -= OnCurrentReached;
            }
        }

        private void OnCurrentReached()
        {
            Reached?.Invoke();
        }
    }
}

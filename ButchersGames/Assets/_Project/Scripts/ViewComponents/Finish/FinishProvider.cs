using System;
using Core.Gameplay.Finish;
using ViewComponents.Level;

namespace ViewComponents.Finish
{
    public sealed class FinishProvider : IFinishProvider
    {
        private readonly LevelProvider _levelProvider;

        private FinishCollider _current;

        public FinishProvider(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;
        }

        public event Action Reached;

        public void StartListening()
        {
            _levelProvider.LevelLoaded += OnLevelLoaded;
        }

        public void StopListening()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;

            UnsubscribeCurrent();
        }

        private void OnLevelLoaded()
        {
            UnsubscribeCurrent();

            _current = _levelProvider.Finish;
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

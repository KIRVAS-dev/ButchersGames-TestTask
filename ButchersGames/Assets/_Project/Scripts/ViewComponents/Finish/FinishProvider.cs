using System;
using Core.Gameplay.Finish;
using ViewComponents.Level;

namespace ViewComponents.Finish
{
    public sealed class FinishProvider
        : IFinishProvider,
          IDisposable
    {
        private readonly LevelProvider _levelProvider;

        private FinishCollider _current;

        public FinishProvider(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;

            _levelProvider.LevelLoaded += OnLevelLoaded;
        }

        public event Action Reached;

        void IDisposable.Dispose()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;

            UnsubscribeCurrent();
        }

        private void OnLevelLoaded()
        {
            UnsubscribeCurrent();

            _current = _levelProvider.CurrentLevel.Finish;
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

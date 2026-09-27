using System;
using UnityEngine;
using UnityEngine.Pool;

namespace UI.FloatingText
{
    internal sealed class FloatingTextSeries
    {
        private readonly ObjectPool<FloatingTextPopup> _pool;
        private readonly FloatingTextConfig _config;
        private readonly float _sideOffset;
        private readonly Action<FloatingTextPopup> _detached;
        private readonly Action<FloatingTextPopup> _release;

        private FloatingTextPopup _activePopup;

        internal event Action Ended;

        internal FloatingTextSeries(
            ObjectPool<FloatingTextPopup> pool,
            FloatingTextConfig config,
            float sideOffset)
        {
            _pool = pool;
            _config = config;
            _sideOffset = sideOffset;
            _detached = OnPopupDetached;
            _release = _pool.Release;
        }

        internal void Show(int amount, Vector2 anchorPosition)
        {
            if (_activePopup != null)
            {
                _activePopup.SetAmount(amount);

                return;
            }

            Vector2 position = anchorPosition;
            position.x += _sideOffset;

            _activePopup = _pool.Get();

            _activePopup.Play(
                amount,
                position,
                _config,
                _detached,
                _release
            );
        }

        private void OnPopupDetached(FloatingTextPopup popup)
        {
            _activePopup = null;
            Ended?.Invoke();
        }
    }
}

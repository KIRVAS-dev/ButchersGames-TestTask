using System.Collections.Generic;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using UnityEngine.Pool;

namespace UI.FloatingText
{
    public sealed class FloatingTextView
        : MonoBehaviour,
          IFloatingTextView
    {
        private const string GainAmountTextFormat = "+{0}";
        private const string LossAmountTextFormat = "-{0}";

        [SerializeField] private Camera _worldCamera;
        [SerializeField] private Transform _anchor;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _container;
        [SerializeField] private FloatingTextPopup _gainPrefab;
        [SerializeField] private FloatingTextPopup _lossPrefab;
        [SerializeField] private FloatingTextConfig _config;

        private ObjectPool<FloatingTextPopup> _gainPool;
        private ObjectPool<FloatingTextPopup> _lossPool;

        private void Awake()
        {
            Validate();

            _config.Validate();

            _gainPool = CreatePool(_gainPrefab);
            _lossPool = CreatePool(_lossPrefab);

            Prewarm(_gainPool);
            Prewarm(_lossPool);
        }

        public void ShowGain(int amount)
        {
            Show(_gainPool, GainAmountTextFormat, amount, _config.SideOffset);
        }

        public void ShowLoss(int amount)
        {
            Show(_lossPool, LossAmountTextFormat, amount, -_config.SideOffset);
        }

        private void Show(
            ObjectPool<FloatingTextPopup> pool,
            string textFormat,
            int amount,
            float sideOffset)
        {
            Vector2 position = ToContainerPoint(_anchor.position);
            position.x += sideOffset;

            FloatingTextPopup floatingText = pool.Get();

            floatingText.Play(
                textFormat,
                amount,
                position,
                _config,
                pool.Release
            );
        }

        private Vector2 ToContainerPoint(Vector3 worldPosition)
        {
            Vector2 screenPoint = _worldCamera.WorldToScreenPoint(worldPosition);

            Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_container, screenPoint, uiCamera, out Vector2 localPoint);

            return localPoint;
        }

        private ObjectPool<FloatingTextPopup> CreatePool(FloatingTextPopup prefab)
        {
            return new ObjectPool<FloatingTextPopup>(
                () => Instantiate(prefab, _container),
                floatingText => floatingText.gameObject.SetActive(true),
                floatingText => floatingText.gameObject.SetActive(false),
                defaultCapacity: _config.PrewarmCount
            );
        }

        private void Prewarm(ObjectPool<FloatingTextPopup> pool)
        {
            List<FloatingTextPopup> prewarmedTexts = new List<FloatingTextPopup>(_config.PrewarmCount);

            for (int i = 0; i < _config.PrewarmCount; i++)
            {
                prewarmedTexts.Add(pool.Get());
            }

            foreach (FloatingTextPopup prewarmedText in prewarmedTexts)
            {
                pool.Release(prewarmedText);
            }
        }

        private void Validate()
        {
            Guard.AgainstNull(_canvas, () => Missing(nameof(_canvas)));
            Guard.AgainstNull(_container, () => Missing(nameof(_container)));
            Guard.AgainstNull(_worldCamera, () => Missing(nameof(_worldCamera)));
            Guard.AgainstNull(_anchor, () => Missing(nameof(_anchor)));
            Guard.AgainstNull(_gainPrefab, () => Missing(nameof(_gainPrefab)));
            Guard.AgainstNull(_lossPrefab, () => Missing(nameof(_lossPrefab)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            return;

            ExtendedException Missing(string fieldName) => new MissingFloatingTextFieldException(fieldName, gameObject.name);
        }
    }
}

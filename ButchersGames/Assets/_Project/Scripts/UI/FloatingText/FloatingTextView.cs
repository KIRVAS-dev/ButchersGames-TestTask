using System;
using System.Collections.Generic;
using ContentValidation;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using UI.RunnerOverlay;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace UI.FloatingText
{
    public sealed class FloatingTextView
        : MonoBehaviour,
          IFloatingTextView,
          IValidatable,
          IWarmupLifecycle
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private FloatingTextPopup _gainPrefab;
        [SerializeField] private FloatingTextPopup _lossPrefab;
        [SerializeField] private FloatingTextConfig _config;

        private IRunnerOverlayTarget _target;
        private FloatingTextSeries _gainSeries;
        private FloatingTextSeries _lossSeries;

        public event Action GainSeriesEnded;
        public event Action LossSeriesEnded;

        [Inject]
        private void Construct(IRunnerOverlayTarget target)
        {
            _target = target;
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_container, () => Missing(nameof(_container)));
            Guard.AgainstNull(_gainPrefab, () => Missing(nameof(_gainPrefab)));
            Guard.AgainstNull(_lossPrefab, () => Missing(nameof(_lossPrefab)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            _config.Validate();
            _gainPrefab.Validate();
            _lossPrefab.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingFloatingTextFieldException(fieldName, gameObject.name);
        }

        void IWarmupLifecycle.Warmup()
        {
            ObjectPool<FloatingTextPopup> gainPool = CreatePool(_gainPrefab);
            ObjectPool<FloatingTextPopup> lossPool = CreatePool(_lossPrefab);

            Prewarm(gainPool);
            Prewarm(lossPool);

            _gainSeries = new FloatingTextSeries(gainPool, _config, _config.CanvasSideOffset);
            _lossSeries = new FloatingTextSeries(lossPool, _config, -_config.CanvasSideOffset);

            _gainSeries.Ended += OnGainSeriesEnded;
            _lossSeries.Ended += OnLossSeriesEnded;
        }

        void IFloatingTextView.ShowGain(int total)
        {
            _gainSeries.Show(total, AnchorPosition());
        }

        void IFloatingTextView.ShowLoss(int total)
        {
            _lossSeries.Show(total, AnchorPosition());
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

        private void OnGainSeriesEnded()
        {
            GainSeriesEnded?.Invoke();
        }

        private void OnLossSeriesEnded()
        {
            LossSeriesEnded?.Invoke();
        }

        private Vector2 AnchorPosition()
        {
            return _target.ContainerPointOf(_config.WorldAnchorOffset, _container);
        }
    }
}

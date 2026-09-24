using ContentValidation;
using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WealthIndicator
{
    public sealed class WealthIndicatorView
        : MonoBehaviour,
          IWealthIndicatorView,
          IValidatable
    {
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private Transform _anchor;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _stageNameText;
        [SerializeField] private Image _fillBarImage;
        [SerializeField] private WealthIndicatorConfig _config;

        private void OnDestroy()
        {
            Canvas.willRenderCanvases -= FollowAnchor;
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_worldCamera, () => Missing(nameof(_worldCamera)));
            Guard.AgainstNull(_anchor, () => Missing(nameof(_anchor)));
            Guard.AgainstNull(_canvas, () => Missing(nameof(_canvas)));
            Guard.AgainstNull(_container, () => Missing(nameof(_container)));
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_stageNameText, () => Missing(nameof(_stageNameText)));
            Guard.AgainstNull(_fillBarImage, () => Missing(nameof(_fillBarImage)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            _config.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingWealthIndicatorFieldException(fieldName, gameObject.name);
        }

        void IWealthIndicatorView.Show()
        {
            _root.gameObject.SetActive(true);
            Canvas.willRenderCanvases += FollowAnchor;
        }

        void IWealthIndicatorView.Hide()
        {
            Canvas.willRenderCanvases -= FollowAnchor;
            _root.gameObject.SetActive(false);
        }

        void IWealthIndicatorView.SetFillBar(float normalizedFill)
        {
            _fillBarImage.fillAmount = normalizedFill;
        }

        void IWealthIndicatorView.SetStage(WealthStage stage)
        {
            WealthStageAppearance appearance = _config.AppearanceOf(stage);

            _stageNameText.text = appearance.DisplayName;
            _stageNameText.color = appearance.Color;
            _fillBarImage.color = appearance.Color;
        }

        private void FollowAnchor()
        {
            _root.anchoredPosition = CanvasPointHelper.WorldToContainerPoint(_anchor.position, _worldCamera, _canvas, _container);
        }
    }
}

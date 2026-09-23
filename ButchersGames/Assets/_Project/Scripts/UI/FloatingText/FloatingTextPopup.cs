using System;
using DG.Tweening;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;

namespace UI.FloatingText
{
    internal sealed class FloatingTextPopup :
        MonoBehaviour,
        IValidatable
    {
        private const float HiddenAlpha = 0f;
        private const float VisibleAlpha = 1f;
        private const float DefaultScale = 1f;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _text;

        private Action<FloatingTextPopup> _completed;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_rectTransform, () => Missing(nameof(_rectTransform)));
            Guard.AgainstNull(_canvasGroup, () => Missing(nameof(_canvasGroup)));
            Guard.AgainstNull(_text, () => Missing(nameof(_text)));

            return;

            ExtendedException Missing(string fieldName) => new MissingFloatingTextFieldException(fieldName, gameObject.name);
        }

        public void Play(
            string textFormat,
            int amount,
            Vector2 anchoredPosition,
            FloatingTextConfig config,
            Action<FloatingTextPopup> completed)
        {
            _completed = completed;

            _rectTransform.DOKill();
            _canvasGroup.DOKill();

            _text.SetText(textFormat, amount);
            _rectTransform.anchoredPosition = anchoredPosition;
            _rectTransform.localScale = Vector3.one * config.AppearStartScale;
            _canvasGroup.alpha = HiddenAlpha;

            float disappearDelay = config.Lifetime - config.DisappearDuration;

            _rectTransform
               .DOAnchorPosY(anchoredPosition.y + config.RiseDistance, config.Lifetime)
               .SetEase(config.MoveEase)
               .SetLink(gameObject);

            _rectTransform.DOScale(DefaultScale, config.AppearDuration).SetEase(config.AppearEase).SetLink(gameObject);

            _canvasGroup.DOFade(VisibleAlpha, config.AppearDuration).SetEase(config.AppearEase).SetLink(gameObject);

            _canvasGroup
               .DOFade(HiddenAlpha, config.DisappearDuration)
               .From(VisibleAlpha, setImmediately: false)
               .SetEase(config.DisappearEase)
               .SetDelay(disappearDelay)
               .SetLink(gameObject)
               .OnComplete(OnDisappeared);
        }

        private void OnDisappeared()
        {
            _completed(this);
        }
    }
}

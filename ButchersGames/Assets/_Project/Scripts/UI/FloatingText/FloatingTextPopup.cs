using System;
using DG.Tweening;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;

namespace UI.FloatingText
{
    internal sealed class FloatingTextPopup
        : MonoBehaviour,
          IValidatable
    {
        private const float HiddenAlpha = 0f;
        private const float VisibleAlpha = 1f;
        private const float DefaultScale = 1f;
        private const int PunchVibrato = 1;
        private const float PunchElasticity = 0f;
        private const string AmountTextFormat = "{0}";

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _text;

        private FloatingTextConfig _config;
        private Action<FloatingTextPopup> _detached;
        private Action<FloatingTextPopup> _completed;
        private Tween _riseTween;
        private Tween _appearScaleTween;
        private Tween _punchTween;
        private Tween _disappearTween;
        private Tween _resumeTween;
        private TweenCallback _resumeLifetime;

        public void Validate()
        {
            Guard.AgainstNull(_rectTransform, () => Missing(nameof(_rectTransform)));
            Guard.AgainstNull(_canvasGroup, () => Missing(nameof(_canvasGroup)));
            Guard.AgainstNull(_text, () => Missing(nameof(_text)));

            return;

            ExtendedException Missing(string fieldName) => new MissingFloatingTextFieldException(fieldName, gameObject.name);
        }

        internal void Play(
            int amount,
            Vector2 anchoredPosition,
            FloatingTextConfig config,
            Action<FloatingTextPopup> detached,
            Action<FloatingTextPopup> completed)
        {
            _config = config;
            _detached = detached;
            _completed = completed;
            _resumeLifetime ??= ResumeLifetime;

            _rectTransform.DOKill();
            _canvasGroup.DOKill();
            _resumeTween?.Kill();

            _text.SetText(AmountTextFormat, amount);
            _rectTransform.anchoredPosition = anchoredPosition;
            _rectTransform.localScale = Vector3.one * config.AppearStartScale;
            _canvasGroup.alpha = HiddenAlpha;

            float disappearDelay = config.Lifetime - config.DisappearDuration;

            _riseTween = _rectTransform
               .DOAnchorPosY(anchoredPosition.y + config.RiseDistance, config.Lifetime)
               .SetEase(config.MoveEase)
               .SetLink(gameObject);

            _appearScaleTween = _rectTransform
               .DOScale(DefaultScale, config.AppearDuration)
               .SetEase(config.AppearEase)
               .SetLink(gameObject);

            _canvasGroup.DOFade(VisibleAlpha, config.AppearDuration).SetEase(config.AppearEase).SetLink(gameObject);

            _disappearTween = _canvasGroup
               .DOFade(HiddenAlpha, config.DisappearDuration)
               .From(VisibleAlpha, setImmediately: false)
               .SetEase(config.DisappearEase)
               .SetDelay(disappearDelay)
               .SetLink(gameObject)
               .OnStart(OnDisappearStarted)
               .OnComplete(OnDisappeared);
        }

        internal void SetAmount(int amount)
        {
            _text.SetText(AmountTextFormat, amount);

            _riseTween.Pause();
            _disappearTween.Pause();
            _resumeTween?.Kill();

            _resumeTween = DOVirtual
               .DelayedCall(_config.AmountChangeStopDuration, _resumeLifetime, ignoreTimeScale: false)
               .SetLink(gameObject);

            PlayPunch();
        }

        private void PlayPunch()
        {
            _appearScaleTween.Complete();
            _punchTween?.Kill();
            _rectTransform.localScale = Vector3.one * DefaultScale;

            _punchTween = _rectTransform
               .DOPunchScale(
                    Vector3.one * _config.AmountChangePunchScale,
                    _config.AmountChangePunchDuration,
                    PunchVibrato,
                    PunchElasticity
                )
               .SetLink(gameObject);
        }

        private void ResumeLifetime()
        {
            _riseTween.Play();
            _disappearTween.Play();
        }

        private void OnDisappearStarted()
        {
            _detached(this);
        }

        private void OnDisappeared()
        {
            _completed(this);
        }
    }
}

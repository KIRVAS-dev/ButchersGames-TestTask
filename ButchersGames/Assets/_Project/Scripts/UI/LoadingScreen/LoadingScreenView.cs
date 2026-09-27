using ContentValidation;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LoadingScreen
{
    public sealed class LoadingScreenView
        : MonoBehaviour,
          ILoadingScreenView,
          IValidatable
    {
        private const int InfiniteLoops = -1;
        private const int MinFrameCount = 2;
        private const float VisibleAlpha = 1f;
        private const float HiddenAlpha = 0f;
        private const char Dot = '.';

        [SerializeField] private LoadingScreenConfig _config;
        [SerializeField] private CanvasGroup _root;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _glow;
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private Sprite[] _frames;

        private int _frameIndex;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_frameImage, () => Missing(nameof(_frameImage)));
            Guard.AgainstNull(_glow, () => Missing(nameof(_glow)));
            Guard.AgainstNull(_loadingText, () => Missing(nameof(_loadingText)));
            Guard.AgainstNullOrEmpty(_frames, () => Missing(nameof(_frames)));
            Guard.AgainstLessThan(_frames.Length, MinFrameCount, () => Invalid(nameof(_frames), _frames.Length));

            foreach (Sprite frame in _frames)
            {
                Guard.AgainstNull(frame, () => Missing(nameof(_frames)));
            }

            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            _config.Validate();

            string dots = new string(Dot, _config.DotCount);

            Guard.AgainstTrue(
                !_loadingText.text.EndsWith(dots),
                () => new InvalidLoadingScreenTextException(nameof(_loadingText), gameObject.name, _config.DotCount)
            );

            return;

            ExtendedException Missing(string fieldName) => new MissingLoadingScreenFieldException(fieldName, gameObject.name);

            ExtendedException Invalid(string fieldName, int value) =>
                new InvalidLoadingScreenValueException(fieldName, gameObject.name, value);
        }

        void ILoadingScreenView.Show()
        {
            KillAnimations();
            _root.alpha = VisibleAlpha;
            _root.gameObject.SetActive(true);

            PlayFrameAnimation();
            PlayGlowAnimation();
            PlayDotsAnimation();
        }

        void ILoadingScreenView.Hide()
        {
            _root.DOKill();

            _root
               .DOFade(HiddenAlpha, _config.FadeOutDuration)
               .SetEase(_config.FadeOutEase)
               .SetLink(gameObject)
               .OnComplete(OnFadedOut);
        }

        private void OnFadedOut()
        {
            KillAnimations();
            _root.gameObject.SetActive(false);
        }

        private void KillAnimations()
        {
            _frameImage.DOKill();
            _glow.DOKill();
            _loadingText.DOKill();
        }

        private void PlayFrameAnimation()
        {
            int lastFrameIndex = _frames.Length - 1;

            SetFrame(0);

            DOVirtual
               .Float(0f, lastFrameIndex, lastFrameIndex * _config.FrameDuration, OnFrameProgress)
               .SetEase(Ease.Linear)
               .SetLoops(InfiniteLoops, LoopType.Yoyo)
               .SetTarget(_frameImage)
               .SetLink(gameObject);
        }

        private void OnFrameProgress(float progress)
        {
            int frameIndex = Mathf.RoundToInt(progress);

            if (frameIndex == _frameIndex)
            {
                return;
            }

            SetFrame(frameIndex);
        }

        private void SetFrame(int frameIndex)
        {
            _frameIndex = frameIndex;
            _frameImage.sprite = _frames[frameIndex];
        }

        private void PlayGlowAnimation()
        {
            Color glowColor = _glow.color;
            glowColor.a = _config.GlowMaxAlpha;
            _glow.color = glowColor;

            _glow
               .DOFade(_config.GlowMinAlpha, _config.GlowPulseDuration)
               .SetEase(_config.GlowPulseEase)
               .SetLoops(InfiniteLoops, LoopType.Yoyo)
               .SetLink(gameObject);
        }

        private void PlayDotsAnimation()
        {
            int dotStepCount = _config.DotCount + 1;

            DOVirtual
               .Float(0f, dotStepCount, dotStepCount * _config.DotInterval, OnDotsProgress)
               .SetEase(Ease.Linear)
               .SetLoops(InfiniteLoops, LoopType.Restart)
               .SetTarget(_loadingText)
               .SetLink(gameObject);
        }

        private void OnDotsProgress(float progress)
        {
            int visibleDotCount = Mathf.Min(Mathf.FloorToInt(progress), _config.DotCount);

            _loadingText.maxVisibleCharacters = _loadingText.text.Length - _config.DotCount + visibleDotCount;
        }
    }
}

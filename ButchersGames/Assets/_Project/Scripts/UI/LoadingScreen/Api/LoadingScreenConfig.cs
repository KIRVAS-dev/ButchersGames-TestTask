using ContentValidation;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace UI.LoadingScreen
{
    [CreateAssetMenu(menuName = "Configs/Loading Screen Config")]
    internal sealed class LoadingScreenConfig
        : ScriptableObject,
          IValidatable
    {
        private const float MaxAlpha = 1f;

        [Header("Frames")]
        [SerializeField] private float _frameDuration = 0.14f;

        [Header("Glow")]
        [SerializeField] private float _glowMinAlpha = 0.35f;
        [SerializeField] private float _glowMaxAlpha = 0.8f;
        [SerializeField] private float _glowPulseDuration = 0.9f;
        [SerializeField] private Ease _glowPulseEase = Ease.InOutSine;

        [Header("Dots")]
        [SerializeField] private int _dotCount = 3;
        [SerializeField] private float _dotInterval = 0.35f;

        [Header("Fade Out")]
        [SerializeField] private float _fadeOutDuration = 0.35f;
        [SerializeField] private Ease _fadeOutEase = Ease.OutQuad;

        public float FrameDuration => _frameDuration;
        public float GlowMinAlpha => _glowMinAlpha;
        public float GlowMaxAlpha => _glowMaxAlpha;
        public float GlowPulseDuration => _glowPulseDuration;
        public Ease GlowPulseEase => _glowPulseEase;

        public int DotCount => _dotCount;
        public float DotInterval => _dotInterval;
        public float FadeOutDuration => _fadeOutDuration;
        public Ease FadeOutEase => _fadeOutEase;

        public void Validate()
        {
            Guard.AgainstNonPositive(_frameDuration, () => Invalid(nameof(_frameDuration), _frameDuration));
            Guard.AgainstNegative(_glowMinAlpha, () => Invalid(nameof(_glowMinAlpha), _glowMinAlpha));
            Guard.AgainstGreaterThan(_glowMinAlpha, MaxAlpha, () => Invalid(nameof(_glowMinAlpha), _glowMinAlpha));
            Guard.AgainstLessThan(_glowMaxAlpha, _glowMinAlpha, () => Invalid(nameof(_glowMaxAlpha), _glowMaxAlpha));
            Guard.AgainstGreaterThan(_glowMaxAlpha, MaxAlpha, () => Invalid(nameof(_glowMaxAlpha), _glowMaxAlpha));
            Guard.AgainstNonPositive(_glowPulseDuration, () => Invalid(nameof(_glowPulseDuration), _glowPulseDuration));
            Guard.AgainstNonPositive(_dotCount, () => Invalid(nameof(_dotCount), _dotCount));
            Guard.AgainstNonPositive(_dotInterval, () => Invalid(nameof(_dotInterval), _dotInterval));
            Guard.AgainstNonPositive(_fadeOutDuration, () => Invalid(nameof(_fadeOutDuration), _fadeOutDuration));

            return;

            ExtendedException Invalid(string fieldName, float value) =>
                new InvalidLoadingScreenConfigValueException(fieldName, value);
        }
    }
}

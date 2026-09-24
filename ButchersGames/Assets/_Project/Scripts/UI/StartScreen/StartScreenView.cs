using System;
using ContentValidation;
using Core.Lifecycle;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StartScreen
{
    public sealed class StartScreenView
        : MonoBehaviour,
          IStartScreenView,
          IValidatable,
          ISubscriptionLifecycle
    {
        private const int InfiniteLoops = -1;

        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private Button _startButton;
        [SerializeField] private RectTransform _hand;
        [SerializeField] private RectTransform _handStartPoint;
        [SerializeField] private RectTransform _handEndPoint;
        [SerializeField] private StartScreenConfig _config;

        public event Action StartClicked;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_levelNumberText, () => Missing(nameof(_levelNumberText)));
            Guard.AgainstNull(_startButton, () => Missing(nameof(_startButton)));
            Guard.AgainstNull(_hand, () => Missing(nameof(_hand)));
            Guard.AgainstNull(_handStartPoint, () => Missing(nameof(_handStartPoint)));
            Guard.AgainstNull(_handEndPoint, () => Missing(nameof(_handEndPoint)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            _config.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingStartScreenFieldException(fieldName, gameObject.name);
        }

        void ISubscriptionLifecycle.Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        void IStartScreenView.Show()
        {
            _root.gameObject.SetActive(true);
            PlayHandAnimation();
        }

        void IStartScreenView.Hide()
        {
            _hand.DOKill();
            _root.gameObject.SetActive(false);
        }

        void IStartScreenView.SetLevelNumber(int levelNumber)
        {
            _levelNumberText.text = LevelNumberTextHelper.Format(levelNumber);
        }

        private void OnStartButtonClicked()
        {
            StartClicked?.Invoke();
        }

        private void PlayHandAnimation()
        {
            _hand.DOKill();
            _hand.anchoredPosition = _handStartPoint.anchoredPosition;

            _hand
               .DOAnchorPos(_handEndPoint.anchoredPosition, _config.HandMoveDuration)
               .SetEase(_config.HandMoveEase)
               .SetLoops(InfiniteLoops, LoopType.Yoyo)
               .SetLink(gameObject);
        }
    }
}

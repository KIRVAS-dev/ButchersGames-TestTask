using System;
using ContentValidation;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ResultScreen
{
    public sealed class ResultScreenView
        : MonoBehaviour,
          IResultScreenView,
          IValidatable,
          ISubscriptionLifecycle
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _winVisualRoot;
        [SerializeField] private RectTransform _loseVisualRoot;
        [SerializeField] private TextMeshProUGUI _moneyAmountText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TextMeshProUGUI _levelNumberText;

        public event Action RetryClicked;
        public event Action NextClicked;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_winVisualRoot, () => Missing(nameof(_winVisualRoot)));
            Guard.AgainstNull(_loseVisualRoot, () => Missing(nameof(_loseVisualRoot)));
            Guard.AgainstNull(_moneyAmountText, () => Missing(nameof(_moneyAmountText)));
            Guard.AgainstNull(_retryButton, () => Missing(nameof(_retryButton)));
            Guard.AgainstNull(_nextButton, () => Missing(nameof(_nextButton)));
            Guard.AgainstNull(_levelNumberText, () => Missing(nameof(_levelNumberText)));

            return;

            ExtendedException Missing(string fieldName) => new MissingResultScreenFieldException(fieldName, gameObject.name);
        }

        void ISubscriptionLifecycle.Start()
        {
            _retryButton.onClick.AddListener(OnRetryButtonClicked);
            _nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            _nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        void IResultScreenView.Show()
        {
            _root.gameObject.SetActive(true);
        }

        void IResultScreenView.Hide()
        {
            _root.gameObject.SetActive(false);
        }

        void IResultScreenView.SetWinResult()
        {
            _winVisualRoot.gameObject.SetActive(true);
            _loseVisualRoot.gameObject.SetActive(false);
        }

        void IResultScreenView.SetLoseResult()
        {
            _winVisualRoot.gameObject.SetActive(false);
            _loseVisualRoot.gameObject.SetActive(true);
        }

        void IResultScreenView.SetLevelNumber(int levelNumber)
        {
            _levelNumberText.text = LevelNumberTextHelper.Format(levelNumber);
        }

        void IResultScreenView.SetMoneyAmount(int amount)
        {
            _moneyAmountText.text = amount.ToString();
        }

        private void OnRetryButtonClicked()
        {
            RetryClicked?.Invoke();
        }

        private void OnNextButtonClicked()
        {
            NextClicked?.Invoke();
        }
    }
}

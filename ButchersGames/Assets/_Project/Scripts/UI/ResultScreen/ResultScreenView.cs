using System;
using ExtendedExceptions;
using Input;
using TMPro;
using UnityEngine;

namespace UI.ResultScreen
{
    public sealed class ResultScreenView
        : MonoBehaviour,
          IResultScreenView,
          IResultScreenInput
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _winVisualRoot;
        [SerializeField] private RectTransform _loseVisualRoot;
        [SerializeField] private TextMeshProUGUI _moneyAmountText;
        [SerializeField] private ButtonClickTrigger _retryButton;
        [SerializeField] private ButtonClickTrigger _nextButton;

        public ITrigger RetryTrigger => _retryButton;
        public ITrigger NextTrigger => _nextButton;

        private void Awake()
        {
            Validate();
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetWinResult()
        {
            _winVisualRoot.gameObject.SetActive(true);
            _loseVisualRoot.gameObject.SetActive(false);
        }

        public void SetLoseResult()
        {
            _winVisualRoot.gameObject.SetActive(false);
            _loseVisualRoot.gameObject.SetActive(true);
        }

        public void SetMoneyAmount(int amount)
        {
            _moneyAmountText.text = amount.ToString();
        }

        private void Validate()
        {
            Func<string, ExtendedException> missing = fieldName =>
                new MissingResultScreenFieldException(fieldName, gameObject.name);

            Guard.AgainstNull(_root, () => missing(nameof(_root)));
            Guard.AgainstNull(_winVisualRoot, () => missing(nameof(_winVisualRoot)));
            Guard.AgainstNull(_loseVisualRoot, () => missing(nameof(_loseVisualRoot)));
            Guard.AgainstNull(_moneyAmountText, () => missing(nameof(_moneyAmountText)));
            Guard.AgainstNull(_retryButton, () => missing(nameof(_retryButton)));
            Guard.AgainstNull(_nextButton, () => missing(nameof(_nextButton)));
        }
    }
}

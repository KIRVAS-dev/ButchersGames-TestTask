using System;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StartScreen
{
    public sealed class StartScreenView
        : MonoBehaviour,
          IStartScreenView
    {
        private const string LevelNumberTextFormat = "Уровень {0}";

        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private Button _startButton;

        public event Action StartClicked;

        private void Awake()
        {
            Validate();

            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }

        public void SetLevelNumber(int levelNumber)
        {
            _levelNumberText.text = string.Format(LevelNumberTextFormat, levelNumber);
        }

        private void OnStartButtonClicked()
        {
            StartClicked?.Invoke();
        }

        private void Validate()
        {
            Func<string, ExtendedException> missing = fieldName =>
                new MissingStartScreenFieldException(fieldName, gameObject.name);

            Guard.AgainstNull(_root, () => missing(nameof(_root)));
            Guard.AgainstNull(_levelNumberText, () => missing(nameof(_levelNumberText)));
            Guard.AgainstNull(_startButton, () => missing(nameof(_startButton)));
        }
    }
}

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

        void IStartScreenView.Show()
        {
            _root.gameObject.SetActive(true);
        }

        void IStartScreenView.Hide()
        {
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

        private void Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_levelNumberText, () => Missing(nameof(_levelNumberText)));
            Guard.AgainstNull(_startButton, () => Missing(nameof(_startButton)));

            return;

            ExtendedException Missing(string fieldName) => new MissingStartScreenFieldException(fieldName, gameObject.name);
        }
    }
}

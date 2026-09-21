using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hud
{
    public sealed class HudView
        : MonoBehaviour,
          IHudView
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private TextMeshProUGUI _moneyAmountText;
        [SerializeField] private TextMeshProUGUI _wealthStageNameText;
        [SerializeField] private Image _wealthFillBarImage;
        [SerializeField] private Image _runProgressFillBarImage;
        [SerializeField] private HudConfig _config;

        private void Awake()
        {
            Validate();

            _config.Validate();
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
            _levelNumberText.text = LevelNumberTextHelper.Format(levelNumber);
        }

        public void SetMoneyAmount(int amount)
        {
            _moneyAmountText.text = amount.ToString();
        }

        public void SetWealthFillBar(float normalizedFill)
        {
            _wealthFillBarImage.fillAmount = normalizedFill;
        }

        public void SetRunProgressFillBar(float normalizedFill)
        {
            _runProgressFillBarImage.fillAmount = normalizedFill;
        }

        public void SetWealthStage(WealthStage stage)
        {
            WealthStageAppearance appearance = _config.AppearanceOf(stage);

            _wealthStageNameText.text = appearance.DisplayName;
            _wealthStageNameText.color = appearance.Color;
            _wealthFillBarImage.color = appearance.Color;
        }

        private void Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_levelNumberText, () => Missing(nameof(_levelNumberText)));
            Guard.AgainstNull(_moneyAmountText, () => Missing(nameof(_moneyAmountText)));
            Guard.AgainstNull(_wealthStageNameText, () => Missing(nameof(_wealthStageNameText)));
            Guard.AgainstNull(_wealthFillBarImage, () => Missing(nameof(_wealthFillBarImage)));
            Guard.AgainstNull(_runProgressFillBarImage, () => Missing(nameof(_runProgressFillBarImage)));
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));

            return;

            ExtendedException Missing(string fieldName) => new MissingHudFieldException(fieldName, gameObject.name);
        }
    }
}

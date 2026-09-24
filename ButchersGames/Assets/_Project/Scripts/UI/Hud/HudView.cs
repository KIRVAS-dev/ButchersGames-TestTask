using ContentValidation;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hud
{
    public sealed class HudView
        : MonoBehaviour,
          IHudView,
          IValidatable
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private TextMeshProUGUI _moneyAmountText;
        [SerializeField] private Image _runProgressFillBarImage;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_root, () => Missing(nameof(_root)));
            Guard.AgainstNull(_levelNumberText, () => Missing(nameof(_levelNumberText)));
            Guard.AgainstNull(_moneyAmountText, () => Missing(nameof(_moneyAmountText)));
            Guard.AgainstNull(_runProgressFillBarImage, () => Missing(nameof(_runProgressFillBarImage)));

            return;

            ExtendedException Missing(string fieldName) => new MissingHudFieldException(fieldName, gameObject.name);
        }

        void IHudView.Show()
        {
            _root.gameObject.SetActive(true);
        }

        void IHudView.Hide()
        {
            _root.gameObject.SetActive(false);
        }

        void IHudView.SetLevelNumber(int levelNumber)
        {
            _levelNumberText.text = LevelNumberTextHelper.Format(levelNumber);
        }

        void IHudView.SetMoneyAmount(int amount)
        {
            _moneyAmountText.text = amount.ToString();
        }

        void IHudView.SetRunProgressFillBar(float normalizedFill)
        {
            _runProgressFillBarImage.fillAmount = normalizedFill;
        }
    }
}

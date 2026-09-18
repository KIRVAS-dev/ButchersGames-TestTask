using System;
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
        [SerializeField] private TextMeshProUGUI _moneyAmountText;
        [SerializeField] private Image _wealthFillBarImage;

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

        public void SetMoneyAmount(int amount)
        {
            _moneyAmountText.text = amount.ToString();
        }

        public void SetWealthFillBar(float normalizedFill)
        {
            _wealthFillBarImage.fillAmount = normalizedFill;
        }

        private void Validate()
        {
            Func<string, ExtendedException> missing = fieldName => new MissingHudFieldException(fieldName, gameObject.name);

            Guard.AgainstNull(_root, () => missing(nameof(_root)));
            Guard.AgainstNull(_moneyAmountText, () => missing(nameof(_moneyAmountText)));
            Guard.AgainstNull(_wealthFillBarImage, () => missing(nameof(_wealthFillBarImage)));
        }
    }
}

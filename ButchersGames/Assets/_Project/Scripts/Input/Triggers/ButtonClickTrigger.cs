using Infrastructure.ExtendedExceptions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Input
{
    public sealed class ButtonClickTrigger
        : MonoBehaviour,
          ITrigger
    {
        [SerializeField] private Button _button;

        public event Action Triggered;

        private void Awake()
        {
            Validate();

            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Triggered?.Invoke();
        }

        private void Validate()
        {
            Guard.AgainstNull(_button, () => new MissingButtonClickTriggerFieldException(nameof(_button), gameObject.name));
        }
    }
}

using ExtendedExceptions;
using Input;
using UnityEngine;

namespace UI.StartScreen
{
    public sealed class StartScreenView
        : MonoBehaviour,
          IStartScreenView,
          IStartScreenInput
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private ButtonClickTrigger _startButton;

        public ITrigger StartTrigger => _startButton;

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

        private void Validate()
        {
            Guard.AgainstNull(_root, () => new MissingStartScreenFieldException(nameof(_root), gameObject.name));
            Guard.AgainstNull(_startButton, () => new MissingStartScreenFieldException(nameof(_startButton), gameObject.name));
        }
    }
}

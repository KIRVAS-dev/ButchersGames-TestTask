using ContentValidation;
using DG.Tweening;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace UI.StartScreen
{
    [CreateAssetMenu(menuName = "Configs/Start Screen Config")]
    internal sealed class StartScreenConfig
        : ScriptableObject,
          IValidatable
    {
        [Header("Tutorial Hand")]
        [SerializeField] private float _handMoveDuration = 0.8f;
        [SerializeField] private Ease _handMoveEase = Ease.InOutSine;

        public float HandMoveDuration => _handMoveDuration;
        public Ease HandMoveEase => _handMoveEase;

        public void Validate()
        {
            Guard.AgainstNonPositive(
                _handMoveDuration,
                () => new InvalidStartScreenValueException(nameof(_handMoveDuration), _handMoveDuration)
            );
        }
    }
}

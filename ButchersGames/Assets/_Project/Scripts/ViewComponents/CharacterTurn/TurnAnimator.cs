using UnityEngine;

namespace ViewComponents.CharacterTurn
{
    internal sealed class TurnAnimator
    {
        private const float NoAngle = 0f;

        private readonly CharacterTurnConfig _config;

        private float _targetAngle;
        private float _releaseTimeLeft;
        private bool _isReleasePending;

        public TurnAnimator(CharacterTurnConfig config)
        {
            _config = config;
        }

        public float CurrentAngle { get; private set; }

        public void SetSide(CharacterTurnSide side)
        {
            switch (side)
            {
                case CharacterTurnSide.None:
                    _isReleasePending = true;
                    _releaseTimeLeft = _config.ReleaseDelay;
                    break;

                case CharacterTurnSide.Left:
                    _targetAngle = -_config.MaxAngle;
                    _isReleasePending = false;
                    break;

                case CharacterTurnSide.Right:
                    _targetAngle = _config.MaxAngle;
                    _isReleasePending = false;
                    break;

                default:
                    throw new UnhandledCharacterTurnSideException(side);
            }
        }

        public void Tick(float deltaTime)
        {
            ReleaseAfterDelay(deltaTime);

            CurrentAngle = Mathf.MoveTowards(CurrentAngle, _targetAngle, _config.TurnSpeed * deltaTime);
        }

        private void ReleaseAfterDelay(float deltaTime)
        {
            if (!_isReleasePending)
            {
                return;
            }

            _releaseTimeLeft -= deltaTime;

            if (_releaseTimeLeft > 0f)
            {
                return;
            }

            _targetAngle = NoAngle;
            _isReleasePending = false;
        }
    }
}

using System;
using System.Threading;
using Core.Gameplay.Obstacle;
using Core.Gameplay.WealthPointsModifier;
using Cysharp.Threading.Tasks;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using ViewComponents.Common;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Obstacles
{
    [RequireComponent(typeof(WealthPointsModifierCollider))]
    public sealed class Obstacle
        : MonoBehaviour,
          ITriggerReaction,
          IObstacle,
          IValidatable
    {
        [SerializeField] private ObstacleConfig _config;

        private WealthPointsModifierCollider _modifier;

        public event Action Hit;
        public event Action Released;

        IWealthPointsModifier IObstacle.Modifier => _modifier;

        private void Awake()
        {
            _modifier = GetComponent<WealthPointsModifierCollider>();
        }

        void IValidatable.Validate()
        {
            WealthPointsModifierCollider modifier = GetComponent<WealthPointsModifierCollider>();

            Guard.AgainstNull(_config, () => new MissingObstacleConfigException(nameof(_config), gameObject.name));
            Guard.AgainstNull(modifier, () => new MissingObstacleModifierColliderException(gameObject.name));

            _config.Validate();
        }

        void ITriggerReaction.React()
        {
            Hit?.Invoke();

            ReleaseAfterDelayAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid ReleaseAfterDelayAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_config.StopDuration), cancellationToken: cancellationToken);

            Released?.Invoke();
        }
    }
}

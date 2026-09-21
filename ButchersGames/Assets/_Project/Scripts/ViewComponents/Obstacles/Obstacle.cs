using System;
using System.Threading;
using Core.Gameplay.Obstacle;
using Core.Gameplay.WealthPointsModifier;
using Cysharp.Threading.Tasks;
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
          IObstacle
    {
        [SerializeField] private ObstacleConfig _config;

        private WealthPointsModifierCollider _modifier;

        public event Action Hit;
        public event Action Released;

        public IWealthPointsModifier Modifier => _modifier;

        private void Awake()
        {
            _modifier = GetComponent<WealthPointsModifierCollider>();

            Validate();
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

        private void Validate()
        {
            Guard.AgainstNull(_config, () => new MissingObstacleConfigException(nameof(_config), gameObject.name));
            Guard.AgainstNull(_modifier, () => new MissingObstacleModifierColliderException(gameObject.name));

            _config.Validate();
        }
    }
}

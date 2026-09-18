using System;
using System.Threading;
using Core.Gameplay.Obstacle;
using Cysharp.Threading.Tasks;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Obstacles
{
    public sealed class Obstacle
        : MonoBehaviour,
          ITriggerReaction,
          IObstacle
    {
        [SerializeField] private ObstacleConfig _config;

        public event Action Hit;
        public event Action Released;

        private void Awake()
        {
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

            _config.Validate();
        }
    }
}

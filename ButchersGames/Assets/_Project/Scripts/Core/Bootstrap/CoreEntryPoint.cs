using System;
using System.Collections.Generic;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using VContainer.Unity;

namespace Core.Bootstrap
{
    internal sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly IReadOnlyList<ISubscriptionLifecycle> _subscriptionLifecycles;

        public CoreEntryPoint(IGameFlowService gameFlowService, IReadOnlyList<ISubscriptionLifecycle> subscriptionLifecycles)
        {
            _gameFlowService = gameFlowService;
            _subscriptionLifecycles = subscriptionLifecycles;
        }

        void IStartable.Start()
        {
            foreach (ISubscriptionLifecycle subscriptionLifecycle in _subscriptionLifecycles)
            {
                subscriptionLifecycle.Start();
            }

            _gameFlowService.PrepareGame();
        }

        void IDisposable.Dispose()
        {
            foreach (ISubscriptionLifecycle subscriptionLifecycle in _subscriptionLifecycles)
            {
                subscriptionLifecycle.Stop();
            }
        }
    }
}

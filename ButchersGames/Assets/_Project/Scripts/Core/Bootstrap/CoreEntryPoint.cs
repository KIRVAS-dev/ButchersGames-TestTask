using System;
using System.Collections.Generic;
using Core.Gameplay.GameFlow;
using Core.Lifecycle;
using Core.Validation;
using VContainer.Unity;

namespace Core.Bootstrap
{
    internal sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly IReadOnlyList<ISubscriptionLifecycle> _subscriptionLifecycles;
        private readonly IReadOnlyList<IWarmupLifecycle> _warmupLifecycles;
        private readonly SessionValidation _sessionValidation;

        public CoreEntryPoint(
            IGameFlowService gameFlowService,
            IReadOnlyList<ISubscriptionLifecycle> subscriptionLifecycles,
            IReadOnlyList<IWarmupLifecycle> warmupLifecycles,
            SessionValidation sessionValidation)
        {
            _gameFlowService = gameFlowService;
            _subscriptionLifecycles = subscriptionLifecycles;
            _warmupLifecycles = warmupLifecycles;
            _sessionValidation = sessionValidation;
        }

        void IStartable.Start()
        {
            _sessionValidation.Validate();

            foreach (IWarmupLifecycle warmupLifecycle in _warmupLifecycles)
            {
                warmupLifecycle.Warmup();
            }

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

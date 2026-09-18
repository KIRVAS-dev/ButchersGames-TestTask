using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly RunnerMovementInputHandler _runnerMovementInputHandler;
        private readonly WealthPointsModifierService _wealthPointsModifierService;
        private readonly GameFlowService _gameFlowService;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            WealthPointsModifierService wealthPointsModifierService,
            GameFlowService gameFlowService)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _wealthPointsModifierService = wealthPointsModifierService;
            _gameFlowService = gameFlowService;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();
            _wealthPointsModifierService.StartListening();
            _gameFlowService.StartListening();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();
            _wealthPointsModifierService.StopListening();
            _gameFlowService.StopListening();
        }
    }
}

using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using VContainer.Unity;
using ViewComponents.RunnerMovement;
using ViewComponents.WealthMeter;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly RunnerMovementInputHandler _runnerMovementInputHandler;
        private readonly WealthPointsModifierService _wealthPointsModifierService;
        private readonly GameFlowService _gameFlowService;
        private readonly CharacterAppearancePresenter _characterAppearancePresenter;
        private readonly RunnerMovementPresenter _runnerMovementPresenter;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            WealthPointsModifierService wealthPointsModifierService,
            GameFlowService gameFlowService,
            CharacterAppearancePresenter characterAppearancePresenter,
            RunnerMovementPresenter runnerMovementPresenter)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _wealthPointsModifierService = wealthPointsModifierService;
            _gameFlowService = gameFlowService;
            _characterAppearancePresenter = characterAppearancePresenter;
            _runnerMovementPresenter = runnerMovementPresenter;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();
            _wealthPointsModifierService.StartListening();
            _gameFlowService.StartListening();
            _characterAppearancePresenter.StartListening();
            _runnerMovementPresenter.StartListening();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();
            _wealthPointsModifierService.StopListening();
            _gameFlowService.StopListening();
            _characterAppearancePresenter.StopListening();
            _runnerMovementPresenter.StopListening();
        }
    }
}

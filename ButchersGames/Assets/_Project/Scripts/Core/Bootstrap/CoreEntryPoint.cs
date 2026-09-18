using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.ResultScreen;
using Core.Input.RunnerMovement;
using Core.Input.StartScreen;
using UI.Hud;
using UI.ResultScreen;
using UI.StartScreen;
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
        private readonly StartScreenInputHandler _startScreenInputHandler;
        private readonly ResultScreenInputHandler _resultScreenInputHandler;
        private readonly WealthPointsModifierService _wealthPointsModifierService;
        private readonly GameFlowService _gameFlowService;
        private readonly CharacterAppearancePresenter _characterAppearancePresenter;
        private readonly RunnerMovementPresenter _runnerMovementPresenter;
        private readonly StartScreenPresenter _startScreenPresenter;
        private readonly HudPresenter _hudPresenter;
        private readonly ResultScreenPresenter _resultScreenPresenter;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            StartScreenInputHandler startScreenInputHandler,
            ResultScreenInputHandler resultScreenInputHandler,
            WealthPointsModifierService wealthPointsModifierService,
            GameFlowService gameFlowService,
            CharacterAppearancePresenter characterAppearancePresenter,
            RunnerMovementPresenter runnerMovementPresenter,
            StartScreenPresenter startScreenPresenter,
            HudPresenter hudPresenter,
            ResultScreenPresenter resultScreenPresenter)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _startScreenInputHandler = startScreenInputHandler;
            _resultScreenInputHandler = resultScreenInputHandler;
            _wealthPointsModifierService = wealthPointsModifierService;
            _gameFlowService = gameFlowService;
            _characterAppearancePresenter = characterAppearancePresenter;
            _runnerMovementPresenter = runnerMovementPresenter;
            _startScreenPresenter = startScreenPresenter;
            _hudPresenter = hudPresenter;
            _resultScreenPresenter = resultScreenPresenter;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();
            _startScreenInputHandler.StartListening();
            _resultScreenInputHandler.StartListening();
            _wealthPointsModifierService.StartListening();
            _characterAppearancePresenter.StartListening();
            _runnerMovementPresenter.StartListening();
            _startScreenPresenter.StartListening();
            _hudPresenter.StartListening();
            _resultScreenPresenter.StartListening();
            _gameFlowService.StartListening();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();
            _startScreenInputHandler.StopListening();
            _resultScreenInputHandler.StopListening();
            _wealthPointsModifierService.StopListening();
            _gameFlowService.StopListening();
            _characterAppearancePresenter.StopListening();
            _runnerMovementPresenter.StopListening();
            _startScreenPresenter.StopListening();
            _hudPresenter.StopListening();
            _resultScreenPresenter.StopListening();
        }
    }
}

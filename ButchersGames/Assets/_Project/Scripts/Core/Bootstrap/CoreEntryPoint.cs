using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using UI.FloatingText;
using UI.Hud;
using UI.ResultScreen;
using UI.StartScreen;
using VContainer.Unity;
using ViewComponents.Feedback;
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
        private readonly StartScreenPresenter _startScreenPresenter;
        private readonly HudPresenter _hudPresenter;
        private readonly ResultScreenPresenter _resultScreenPresenter;
        private readonly FeedbackPresenter _feedbackPresenter;
        private readonly FloatingTextPresenter _floatingTextPresenter;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            WealthPointsModifierService wealthPointsModifierService,
            GameFlowService gameFlowService,
            CharacterAppearancePresenter characterAppearancePresenter,
            RunnerMovementPresenter runnerMovementPresenter,
            StartScreenPresenter startScreenPresenter,
            HudPresenter hudPresenter,
            ResultScreenPresenter resultScreenPresenter,
            FeedbackPresenter feedbackPresenter,
            FloatingTextPresenter floatingTextPresenter)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _wealthPointsModifierService = wealthPointsModifierService;
            _gameFlowService = gameFlowService;
            _characterAppearancePresenter = characterAppearancePresenter;
            _runnerMovementPresenter = runnerMovementPresenter;
            _startScreenPresenter = startScreenPresenter;
            _hudPresenter = hudPresenter;
            _resultScreenPresenter = resultScreenPresenter;
            _feedbackPresenter = feedbackPresenter;
            _floatingTextPresenter = floatingTextPresenter;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();
            _wealthPointsModifierService.StartListening();
            _characterAppearancePresenter.StartListening();
            _runnerMovementPresenter.StartListening();
            _startScreenPresenter.StartListening();
            _hudPresenter.StartListening();
            _resultScreenPresenter.StartListening();
            _feedbackPresenter.StartListening();
            _floatingTextPresenter.StartListening();
            _gameFlowService.StartListening();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();
            _wealthPointsModifierService.StopListening();
            _gameFlowService.StopListening();
            _characterAppearancePresenter.StopListening();
            _runnerMovementPresenter.StopListening();
            _startScreenPresenter.StopListening();
            _hudPresenter.StopListening();
            _resultScreenPresenter.StopListening();
            _feedbackPresenter.StopListening();
            _floatingTextPresenter.StopListening();
        }
    }
}

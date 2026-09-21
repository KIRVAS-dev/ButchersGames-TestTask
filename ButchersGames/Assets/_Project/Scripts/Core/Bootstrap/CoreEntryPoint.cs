using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using Core.Gameplay.WealthMeter;
using Core.Gameplay.WealthPointsModifier;
using Core.Input.RunnerMovement;
using UI.FloatingText;
using UI.Hud;
using UI.ResultScreen;
using UI.StartScreen;
using VContainer.Unity;
using ViewComponents.CharacterAnimation;
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
        private readonly RunnerMovementService _runnerMovementService;
        private readonly WealthMeterService _wealthMeterService;
        private readonly WealthPointsModifierService _wealthPointsModifierService;
        private readonly GameResultDetector _gameResultDetector;
        private readonly GameFlowService _gameFlowService;
        private readonly CharacterAppearancePresenter _characterAppearancePresenter;
        private readonly RunnerMovementPresenter _runnerMovementPresenter;
        private readonly CharacterAnimationPresenter _characterAnimationPresenter;
        private readonly StartScreenPresenter _startScreenPresenter;
        private readonly HudPresenter _hudPresenter;
        private readonly ResultScreenPresenter _resultScreenPresenter;
        private readonly FeedbackPresenter _feedbackPresenter;
        private readonly FloatingTextPresenter _floatingTextPresenter;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            RunnerMovementService runnerMovementService,
            WealthMeterService wealthMeterService,
            WealthPointsModifierService wealthPointsModifierService,
            GameResultDetector gameResultDetector,
            GameFlowService gameFlowService,
            CharacterAppearancePresenter characterAppearancePresenter,
            RunnerMovementPresenter runnerMovementPresenter,
            CharacterAnimationPresenter characterAnimationPresenter,
            StartScreenPresenter startScreenPresenter,
            HudPresenter hudPresenter,
            ResultScreenPresenter resultScreenPresenter,
            FeedbackPresenter feedbackPresenter,
            FloatingTextPresenter floatingTextPresenter)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _runnerMovementService = runnerMovementService;
            _wealthMeterService = wealthMeterService;
            _wealthPointsModifierService = wealthPointsModifierService;
            _gameResultDetector = gameResultDetector;
            _gameFlowService = gameFlowService;
            _characterAppearancePresenter = characterAppearancePresenter;
            _runnerMovementPresenter = runnerMovementPresenter;
            _characterAnimationPresenter = characterAnimationPresenter;
            _startScreenPresenter = startScreenPresenter;
            _hudPresenter = hudPresenter;
            _resultScreenPresenter = resultScreenPresenter;
            _feedbackPresenter = feedbackPresenter;
            _floatingTextPresenter = floatingTextPresenter;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();

            _gameResultDetector.StartListening();
            _runnerMovementService.StartListening();
            _wealthMeterService.StartListening();
            _wealthPointsModifierService.StartListening();

            _characterAppearancePresenter.StartListening();
            _runnerMovementPresenter.StartListening();
            _characterAnimationPresenter.StartListening();
            _startScreenPresenter.StartListening();
            _hudPresenter.StartListening();
            _resultScreenPresenter.StartListening();
            _feedbackPresenter.StartListening();
            _floatingTextPresenter.StartListening();

            _gameFlowService.PrepareGame();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();

            _gameResultDetector.StopListening();
            _runnerMovementService.StopListening();
            _wealthMeterService.StopListening();
            _wealthPointsModifierService.StopListening();

            _characterAppearancePresenter.StopListening();
            _runnerMovementPresenter.StopListening();
            _characterAnimationPresenter.StopListening();
            _startScreenPresenter.StopListening();
            _hudPresenter.StopListening();
            _resultScreenPresenter.StopListening();
            _feedbackPresenter.StopListening();
            _floatingTextPresenter.StopListening();
        }
    }
}

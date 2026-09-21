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
using ViewComponents.CharacterTurn;
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
        private readonly WealthMeterService _wealthMeterService;
        private readonly WealthPointsModifierService _wealthPointsModifierService;
        private readonly RunnerMovementService _runnerMovementService;
        private readonly RunnerMovementPresenter _runnerMovementPresenter;
        private readonly CharacterAppearancePresenter _characterAppearancePresenter;
        private readonly CharacterAnimationPresenter _characterAnimationPresenter;
        private readonly CharacterTurnPresenter _characterTurnPresenter;
        private readonly FeedbackPresenter _feedbackPresenter;
        private readonly FloatingTextPresenter _floatingTextPresenter;
        private readonly StartScreenPresenter _startScreenPresenter;
        private readonly HudPresenter _hudPresenter;
        private readonly ResultScreenPresenter _resultScreenPresenter;
        private readonly GameResultDetector _gameResultDetector;
        private readonly GameFlowService _gameFlowService;

        public CoreEntryPoint(
            RunnerMovementInputHandler runnerMovementInputHandler,
            WealthMeterService wealthMeterService,
            WealthPointsModifierService wealthPointsModifierService,
            RunnerMovementService runnerMovementService,
            RunnerMovementPresenter runnerMovementPresenter,
            CharacterAppearancePresenter characterAppearancePresenter,
            CharacterAnimationPresenter characterAnimationPresenter,
            CharacterTurnPresenter characterTurnPresenter,
            FeedbackPresenter feedbackPresenter,
            FloatingTextPresenter floatingTextPresenter,
            StartScreenPresenter startScreenPresenter,
            HudPresenter hudPresenter,
            ResultScreenPresenter resultScreenPresenter,
            GameResultDetector gameResultDetector,
            GameFlowService gameFlowService)
        {
            _runnerMovementInputHandler = runnerMovementInputHandler;
            _wealthMeterService = wealthMeterService;
            _wealthPointsModifierService = wealthPointsModifierService;
            _runnerMovementService = runnerMovementService;
            _runnerMovementPresenter = runnerMovementPresenter;
            _characterAppearancePresenter = characterAppearancePresenter;
            _characterAnimationPresenter = characterAnimationPresenter;
            _characterTurnPresenter = characterTurnPresenter;
            _feedbackPresenter = feedbackPresenter;
            _floatingTextPresenter = floatingTextPresenter;
            _startScreenPresenter = startScreenPresenter;
            _hudPresenter = hudPresenter;
            _resultScreenPresenter = resultScreenPresenter;
            _gameResultDetector = gameResultDetector;
            _gameFlowService = gameFlowService;
        }

        void IStartable.Start()
        {
            _runnerMovementInputHandler.StartListening();

            _wealthMeterService.StartListening();
            _wealthPointsModifierService.StartListening();
            _runnerMovementService.StartListening();

            _runnerMovementPresenter.StartListening();
            _characterAppearancePresenter.StartListening();
            _characterAnimationPresenter.StartListening();
            _characterTurnPresenter.StartListening();
            _feedbackPresenter.StartListening();
            _floatingTextPresenter.StartListening();
            _startScreenPresenter.StartListening();
            _hudPresenter.StartListening();
            _resultScreenPresenter.StartListening();

            _gameResultDetector.StartListening();

            _gameFlowService.PrepareGame();
        }

        void IDisposable.Dispose()
        {
            _runnerMovementInputHandler.StopListening();

            _runnerMovementService.StopListening();
            _wealthMeterService.StopListening();
            _wealthPointsModifierService.StopListening();

            _runnerMovementPresenter.StopListening();
            _characterAppearancePresenter.StopListening();
            _characterAnimationPresenter.StopListening();
            _characterTurnPresenter.StopListening();
            _feedbackPresenter.StopListening();
            _floatingTextPresenter.StopListening();
            _startScreenPresenter.StopListening();
            _hudPresenter.StopListening();
            _resultScreenPresenter.StopListening();

            _gameResultDetector.StopListening();
        }
    }
}

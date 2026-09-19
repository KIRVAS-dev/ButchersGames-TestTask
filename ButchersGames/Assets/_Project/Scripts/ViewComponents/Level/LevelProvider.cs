using System;
using System.Collections.Generic;
using Core.Gameplay.LevelProgression;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using ViewComponents.Finish;
using ViewComponents.LaneBarriers;
using ViewComponents.Obstacles;
using ViewComponents.Track;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Level
{
    public sealed class LevelProvider
        : MonoBehaviour,
          ILevelProvider
    {
        [SerializeField] private LevelListConfig _levelListConfig;

        private Level _currentLevel;

        public event Action LevelLoaded;

        public int LevelCount => _levelListConfig.Levels.Count;
        public bool IsRandomized => _levelListConfig.IsRandomized;
        public IReadOnlyCollection<Obstacle> Obstacles => CurrentLevel.Obstacles;
        public IReadOnlyCollection<LaneBarrierZone> Barriers => CurrentLevel.Barriers;
        public IReadOnlyCollection<WealthPointsModifierCollider> Modifiers => CurrentLevel.Modifiers;
        public FinishCollider Finish => CurrentLevel.Finish;
        public float TrackLength => CurrentLevel.Track.Length;

        private void Awake()
        {
            Validate();
        }

        public Level LevelAt(int levelIndex)
        {
            return _levelListConfig.Levels[levelIndex];
        }

        public TrackPoint TrackPointAt(float distance)
        {
            return CurrentLevel.Track.PointAt(distance);
        }

        public void NotifyLevelLoaded(Level level)
        {
            _currentLevel = level;

            LevelLoaded?.Invoke();
        }

        private Level CurrentLevel
        {
            get
            {
                Guard.AgainstNull(_currentLevel, () => new LevelNotLoadedException(gameObject.name));

                return _currentLevel;
            }
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _levelListConfig,
                () => new MissingLevelListConfigException(nameof(_levelListConfig), gameObject.name)
            );

            _levelListConfig.Validate();
        }
    }
}

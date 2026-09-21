using System;
using System.Collections.Generic;
using Core.Gameplay.Feedback;
using FMODUnity;
using UnityEngine;

namespace ViewComponents.Feedback
{
    [Serializable]
    internal sealed class FeedbackEntry
    {
        [SerializeField] private FeedbackType _type;
        [SerializeField] private EventReference _sound;
        [SerializeField] private ParticleSystem[] _particles;

        public FeedbackType Type => _type;
        public EventReference Sound => _sound;
        public IReadOnlyList<ParticleSystem> Particles => _particles;
    }
}

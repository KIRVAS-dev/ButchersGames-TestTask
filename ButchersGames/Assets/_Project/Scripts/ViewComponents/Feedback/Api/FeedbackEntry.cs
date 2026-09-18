using System;
using Core.Gameplay.Feedback;
using FMODUnity;
using UnityEngine;

namespace ViewComponents.Feedback
{
    [Serializable]
    public sealed class FeedbackEntry
    {
        [SerializeField] private FeedbackType _type;
        [SerializeField] private EventReference _sound;
        [SerializeField] private ParticleSystem[] _particles;

        public FeedbackType Type => _type;
        public EventReference Sound => _sound;
        public ParticleSystem[] Particles => _particles;
    }
}

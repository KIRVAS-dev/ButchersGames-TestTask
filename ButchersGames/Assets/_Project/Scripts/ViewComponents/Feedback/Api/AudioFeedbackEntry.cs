using System;
using Core.Gameplay.Feedback;
using FMODUnity;
using UnityEngine;

namespace ViewComponents.Feedback
{
    [Serializable]
    internal sealed class AudioFeedbackEntry
    {
        [SerializeField] private AudioFeedbackType _type;
        [SerializeField] private EventReference _sound;

        public AudioFeedbackType Type => _type;
        public EventReference Sound => _sound;
    }
}

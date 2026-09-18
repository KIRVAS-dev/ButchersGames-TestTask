using System.Collections.Generic;
using Core.Gameplay.Feedback;
using FMODUnity;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Feedback
{
    public sealed class FeedbackPerformer
        : MonoBehaviour,
          IFeedbackPerformer
    {
        [SerializeField] private List<FeedbackEntry> _entries;

        private readonly Dictionary<FeedbackType, FeedbackEntry> _entriesByType = new Dictionary<FeedbackType, FeedbackEntry>();

        private void Awake()
        {
            Validate();

            foreach (FeedbackEntry entry in _entries)
            {
                _entriesByType.Add(entry.Type, entry);
            }
        }

        public void Play(FeedbackType type)
        {
            if (!_entriesByType.TryGetValue(type, out FeedbackEntry entry))
            {
                return;
            }

            if (!entry.Sound.IsNull)
            {
                RuntimeManager.PlayOneShot(entry.Sound);
            }

            foreach (ParticleSystem particle in entry.Particles)
            {
                if (particle == null)
                {
                    continue;
                }

                particle.Stop();
                particle.Play();
            }
        }

        private void Validate()
        {
            HashSet<FeedbackType> seenTypes = new HashSet<FeedbackType>();

            foreach (FeedbackEntry entry in _entries)
            {
                Guard.AgainstTrue(
                    !seenTypes.Add(entry.Type),
                    () => new DuplicateFeedbackEntryException(entry.Type, gameObject.name)
                );
            }
        }
    }
}

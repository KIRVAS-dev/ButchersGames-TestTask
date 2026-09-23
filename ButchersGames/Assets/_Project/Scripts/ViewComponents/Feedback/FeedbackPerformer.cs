using System.Collections.Generic;
using ContentValidation;
using Core.Gameplay.Feedback;
using Core.Lifecycle;
using FMODUnity;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Feedback
{
    public sealed class FeedbackPerformer
        : MonoBehaviour,
          IFeedbackPerformer,
          IValidatable,
          IWarmupLifecycle
    {
        [SerializeField] private List<FeedbackEntry> _entries;

        private readonly Dictionary<FeedbackType, FeedbackEntry> _entriesByType = new Dictionary<FeedbackType, FeedbackEntry>();

        void IValidatable.Validate()
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

        void IWarmupLifecycle.Warmup()
        {
            foreach (FeedbackEntry entry in _entries)
            {
                _entriesByType.Add(entry.Type, entry);
            }
        }

        void IFeedbackPerformer.Play(FeedbackType type)
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
    }
}

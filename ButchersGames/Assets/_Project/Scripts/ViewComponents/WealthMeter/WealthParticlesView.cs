using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthMeter
{
    [DisallowMultipleComponent]
    public sealed class WealthParticlesView
        : MonoBehaviour,
          IWealthParticlesView,
          IValidatable
    {
        [SerializeField] private ParticleSystem _increaseParticles;
        [SerializeField] private ParticleSystem _decreaseParticles;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_increaseParticles, () => Missing(nameof(_increaseParticles)));
            Guard.AgainstNull(_decreaseParticles, () => Missing(nameof(_decreaseParticles)));

            return;

            ExtendedException Missing(string fieldName) =>
                new MissingWealthParticlesViewFieldException(fieldName, gameObject.name);
        }

        void IWealthParticlesView.PlayIncrease()
        {
            Restart(_increaseParticles);
        }

        void IWealthParticlesView.PlayDecrease()
        {
            Restart(_decreaseParticles);
        }

        private static void Restart(ParticleSystem particles)
        {
            particles.Stop();
            particles.Play();
        }
    }
}

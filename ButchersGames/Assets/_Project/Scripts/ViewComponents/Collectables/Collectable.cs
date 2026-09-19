using Infrastructure.ExtendedExceptions;
using UnityEngine;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Collectables
{
    [RequireComponent(typeof(WealthPointsModifierCollider))]
    public sealed class Collectable
        : MonoBehaviour,
          ITriggerReaction
    {
        private void Awake()
        {
            Validate();
        }

        void ITriggerReaction.React() => Collect();

        private void Collect()
        {
            gameObject.SetActive(false);
        }

        private void Validate()
        {
            Guard.AgainstNull(
                GetComponent<WealthPointsModifierCollider>(),
                () => new MissingCollectableModifierColliderException(gameObject.name)
            );
        }
    }
}

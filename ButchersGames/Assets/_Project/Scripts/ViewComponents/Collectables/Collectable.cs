using UnityEngine;
using ViewComponents.Common;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Collectables
{
    [RequireComponent(typeof(WealthPointsModifierCollider))]
    public sealed class Collectable
        : MonoBehaviour,
          ITriggerReaction
    {
        void ITriggerReaction.React() => Collect();

        private void Collect()
        {
            gameObject.SetActive(false);
        }
    }
}

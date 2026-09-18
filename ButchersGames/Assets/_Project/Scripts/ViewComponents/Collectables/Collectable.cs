using UnityEngine;

namespace ViewComponents.Collectables
{
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

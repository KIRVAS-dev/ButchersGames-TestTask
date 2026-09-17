using UnityEngine;

namespace ViewComponents.Collectables
{
    public sealed class Collectable : MonoBehaviour
    {
        public void Collect()
        {
            gameObject.SetActive(false);
        }
    }
}

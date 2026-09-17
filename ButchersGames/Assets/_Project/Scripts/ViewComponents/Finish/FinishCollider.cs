using System;
using UnityEngine;

namespace ViewComponents.Finish
{
    [RequireComponent(typeof(Collider))]
    public sealed class FinishCollider : MonoBehaviour
    {
        private bool _isTriggered;

        public event Action Reached;

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered)
            {
                return;
            }

            _isTriggered = true;

            Reached?.Invoke();
        }
    }
}

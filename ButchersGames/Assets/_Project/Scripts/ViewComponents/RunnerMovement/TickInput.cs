using System;
using Input;
using UnityEngine;

namespace ViewComponents.RunnerMovement
{
    [DefaultExecutionOrder(-100)]
    public sealed class TickInput
        : MonoBehaviour,
          ITickInput
    {
        public event Action<float> Ticked;

        private void Update()
        {
            Ticked?.Invoke(Time.deltaTime);
        }
    }
}

using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ViewComponents.RunnerMovement
{
    [DefaultExecutionOrder(-200)]
    public sealed class DragInput : MonoBehaviour, IDragInput
    {
        private const float ScreenHalfFactor = 0.5f;

        public event Action<float> DragNormalizedOffsetChanged;

        private void Update()
        {
            Pointer pointer = Pointer.current;
            bool isPointerPressed = pointer != null && pointer.press.isPressed;

            if (!isPointerPressed)
            {
                return;
            }

            float screenHalfWidth = Screen.width * ScreenHalfFactor;
            float normalizedOffset = (pointer.position.ReadValue().x - screenHalfWidth) / screenHalfWidth;

            DragNormalizedOffsetChanged?.Invoke(normalizedOffset);
        }
    }
}

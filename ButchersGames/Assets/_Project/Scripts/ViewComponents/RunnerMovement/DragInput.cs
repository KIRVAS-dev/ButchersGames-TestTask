using System;
using Core;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ViewComponents.RunnerMovement
{
    public sealed class DragInput
        : MonoBehaviour,
          IDragInput,
          IInputTickable
    {
        private const float ScreenHalfFactor = 0.5f;

        public event Action<float> DragNormalizedOffsetChanged;

        void IInputTickable.Tick()
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

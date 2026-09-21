using System;
using Core.Input;
using Core.Loop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class DragInput
        : IDragInput,
          IInputTickable
    {
        private const float ScreenHalfFactor = 0.5f;

        private bool _wasPressed;
        private float _previousPointerX;

        public event Action<float> DragNormalizedDeltaChanged;

        void IInputTickable.Tick()
        {
            Pointer pointer = Pointer.current;
            bool isPointerPressed = pointer != null && pointer.press.isPressed;

            if (!isPointerPressed)
            {
                _wasPressed = false;
                return;
            }

            float pointerX = pointer.position.ReadValue().x;

            if (!_wasPressed)
            {
                _wasPressed = true;
                _previousPointerX = pointerX;
                return;
            }

            float screenHalfWidth = Screen.width * ScreenHalfFactor;
            float normalizedDelta = (pointerX - _previousPointerX) / screenHalfWidth;

            _previousPointerX = pointerX;

            if (normalizedDelta != 0f)
            {
                DragNormalizedDeltaChanged?.Invoke(normalizedDelta);
            }
        }
    }
}

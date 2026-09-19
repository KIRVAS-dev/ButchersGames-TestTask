using System;
using Core.Gameplay.WealthMeter;
using UnityEngine;

namespace UI.Hud
{
    [Serializable]
    public sealed class WealthStageAppearance
    {
        [SerializeField] private WealthStage _stage;
        [SerializeField] private string _displayName;
        [SerializeField] private Color _color = Color.white;

        public WealthStage Stage => _stage;
        public string DisplayName => _displayName;
        public Color Color => _color;
    }
}

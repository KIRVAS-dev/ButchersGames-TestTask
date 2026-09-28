using System;
using UnityEngine;

namespace ViewComponents.EnvironmentTheme
{
    [Serializable]
    internal sealed class EnvironmentThemeEntry
    {
        [SerializeField] private Texture2D _sky;
        [SerializeField] private Texture2D _sea;
        [SerializeField] private Color _fogColor = Color.white;
        [SerializeField] private Color _ambientColor = Color.gray;

        internal Texture2D Sky => _sky;
        internal Texture2D Sea => _sea;
        internal Color FogColor => _fogColor;
        internal Color AmbientColor => _ambientColor;
    }
}

using System.Collections.Generic;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TrackColor
{
    [CreateAssetMenu(menuName = "Configs/Track Palette Config")]
    internal sealed class TrackPaletteConfig
        : ScriptableObject,
          IValidatable
    {
        [SerializeField] private List<Color> _colors;

        internal IReadOnlyList<Color> Colors => _colors;

        public void Validate()
        {
            Guard.AgainstNullOrEmpty(_colors, () => new EmptyTrackPaletteException(name));
        }
    }
}

using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TrackColor
{
    [DisallowMultipleComponent]
    public sealed class TrackColorView
        : MonoBehaviour,
          ITrackColorView,
          IValidatable
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private TrackPaletteConfig _palette;

        private MaterialPropertyBlock _propertyBlock;

        int ITrackColorView.ColorCount => _palette.Colors.Count;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_renderer, () => Missing(nameof(_renderer)));
            Guard.AgainstNull(_palette, () => Missing(nameof(_palette)));

            _palette.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingTrackColorFieldException(fieldName, gameObject.name);
        }

        void ITrackColorView.SetColor(int index)
        {
            _propertyBlock ??= new MaterialPropertyBlock();

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(BaseColorId, _palette.Colors[index]);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}

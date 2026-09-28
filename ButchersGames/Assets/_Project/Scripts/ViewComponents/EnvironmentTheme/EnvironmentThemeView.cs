using ContentValidation;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.EnvironmentTheme
{
    [DisallowMultipleComponent]
    public sealed class EnvironmentThemeView
        : MonoBehaviour,
          IEnvironmentThemeView,
          IValidatable,
          IWarmupLifecycle
    {
        private static readonly int SkyTextureId = Shader.PropertyToID("_MainTex");
        private static readonly int SeaTextureId = Shader.PropertyToID("_BaseMap");

        [SerializeField] private EnvironmentThemeConfig _config;
        [SerializeField] private Material _skyboxMaterial;
        [SerializeField] private MeshRenderer _seaRenderer;

        private Material _skybox;
        private MaterialPropertyBlock _seaPropertyBlock;

        int IEnvironmentThemeView.ThemeCount => _config.Themes.Count;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_config, () => Missing(nameof(_config)));
            Guard.AgainstNull(_skyboxMaterial, () => Missing(nameof(_skyboxMaterial)));
            Guard.AgainstNull(_seaRenderer, () => Missing(nameof(_seaRenderer)));

            _config.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingEnvironmentThemeFieldException(fieldName, gameObject.name);
        }

        void IWarmupLifecycle.Warmup()
        {
            _skybox = new Material(_skyboxMaterial);
            _seaPropertyBlock = new MaterialPropertyBlock();

            RenderSettings.skybox = _skybox;
        }

        void IEnvironmentThemeView.SetTheme(int index)
        {
            EnvironmentThemeEntry theme = _config.Themes[index];

            _skybox.SetTexture(SkyTextureId, theme.Sky);

            _seaRenderer.GetPropertyBlock(_seaPropertyBlock);
            _seaPropertyBlock.SetTexture(SeaTextureId, theme.Sea);
            _seaRenderer.SetPropertyBlock(_seaPropertyBlock);

            RenderSettings.fogColor = theme.FogColor;
            RenderSettings.ambientLight = theme.AmbientColor;
        }

        private void OnDestroy()
        {
            Destroy(_skybox);
        }
    }
}

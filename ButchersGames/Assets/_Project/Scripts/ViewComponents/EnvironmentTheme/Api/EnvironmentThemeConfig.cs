using System.Collections.Generic;
using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.EnvironmentTheme
{
    [CreateAssetMenu(menuName = "Configs/Environment Theme Config")]
    internal sealed class EnvironmentThemeConfig
        : ScriptableObject,
          IValidatable
    {
        [SerializeField] private List<EnvironmentThemeEntry> _themes;

        internal IReadOnlyList<EnvironmentThemeEntry> Themes => _themes;

        public void Validate()
        {
            Guard.AgainstNullOrEmpty(_themes, () => new EmptyEnvironmentThemeConfigException(name));

            for (int i = 0; i < _themes.Count; i++)
            {
                int index = i;
                EnvironmentThemeEntry theme = _themes[i];

                Guard.AgainstNull(theme.Sky, () => MissingTexture(index, nameof(EnvironmentThemeEntry.Sky)));
                Guard.AgainstNull(theme.Sea, () => MissingTexture(index, nameof(EnvironmentThemeEntry.Sea)));
            }

            return;

            ExtendedException MissingTexture(int index, string textureName) =>
                new MissingEnvironmentThemeTextureException(name, index, textureName);
        }
    }
}

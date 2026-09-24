using System;
using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace UI.WealthIndicator
{
    [CreateAssetMenu(menuName = "Configs/Wealth Indicator Config")]
    internal sealed class WealthIndicatorConfig
        : ScriptableObject,
          IValidatable
    {
        [SerializeField] private WealthStageAppearance[] _stageAppearances;

        public WealthStageAppearance AppearanceOf(WealthStage stage)
        {
            foreach (WealthStageAppearance appearance in _stageAppearances)
            {
                if (appearance.Stage == stage)
                {
                    return appearance;
                }
            }

            throw new InvalidWealthIndicatorStageAppearanceCountException(stage, CountOf(stage));
        }

        public void Validate()
        {
            foreach (WealthStage stage in Enum.GetValues(typeof(WealthStage)))
            {
                int count = CountOf(stage);

                Guard.AgainstTrue(count != 1, () => new InvalidWealthIndicatorStageAppearanceCountException(stage, count));
            }

            foreach (WealthStageAppearance appearance in _stageAppearances)
            {
                Guard.AgainstTrue(
                    string.IsNullOrEmpty(appearance.DisplayName),
                    () => new EmptyWealthIndicatorStageNameException(appearance.Stage)
                );
            }
        }

        private int CountOf(WealthStage stage)
        {
            int count = 0;

            foreach (WealthStageAppearance appearance in _stageAppearances)
            {
                if (appearance.Stage == stage)
                {
                    count++;
                }
            }

            return count;
        }
    }
}

using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthMeter
{
    public sealed class CharacterAppearanceView
        : MonoBehaviour,
          ICharacterAppearanceView
    {
        [SerializeField] private GameObject _poor;
        [SerializeField] private GameObject _casual;
        [SerializeField] private GameObject _middle;
        [SerializeField] private GameObject _business;
        [SerializeField] private GameObject _rich;

        private void Awake()
        {
            Validate();
        }

        public void SetActiveStage(WealthStage stage)
        {
            _poor.SetActive(stage == WealthStage.Poor);
            _casual.SetActive(stage == WealthStage.Casual);
            _middle.SetActive(stage == WealthStage.Middle);
            _business.SetActive(stage == WealthStage.Business);
            _rich.SetActive(stage == WealthStage.Rich);
        }

        private void Validate()
        {
            Guard.AgainstNull(_poor, () => Missing(nameof(_poor)));
            Guard.AgainstNull(_casual, () => Missing(nameof(_casual)));
            Guard.AgainstNull(_middle, () => Missing(nameof(_middle)));
            Guard.AgainstNull(_business, () => Missing(nameof(_business)));
            Guard.AgainstNull(_rich, () => Missing(nameof(_rich)));

            return;

            ExtendedException Missing(string fieldName) =>
                new MissingCharacterAppearanceViewFieldException(fieldName, gameObject.name);
        }
    }
}

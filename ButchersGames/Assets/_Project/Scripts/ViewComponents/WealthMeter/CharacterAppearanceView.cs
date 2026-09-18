using System;
using Core.Gameplay.WealthMeter;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthMeter
{
    public sealed class CharacterAppearanceView
        : MonoBehaviour,
          ICharacterAppearanceView
    {
        [SerializeField] private GameObject _poor;
        [SerializeField] private GameObject _descent;
        [SerializeField] private GameObject _casual;
        [SerializeField] private GameObject _rich;
        [SerializeField] private GameObject _millionaire;

        private void Awake()
        {
            Validate();
        }

        public void SetActiveStage(WealthStage stage)
        {
            _poor.SetActive(stage == WealthStage.Poor);
            _descent.SetActive(stage == WealthStage.Descent);
            _casual.SetActive(stage == WealthStage.Casual);
            _rich.SetActive(stage == WealthStage.Rich);
            _millionaire.SetActive(stage == WealthStage.Millionaire);
        }

        private void Validate()
        {
            Func<string, ExtendedException> missing = fieldName =>
                new MissingCharacterAppearanceViewFieldException(fieldName, gameObject.name);

            Guard.AgainstNull(_poor, () => missing(nameof(_poor)));
            Guard.AgainstNull(_descent, () => missing(nameof(_descent)));
            Guard.AgainstNull(_casual, () => missing(nameof(_casual)));
            Guard.AgainstNull(_rich, () => missing(nameof(_rich)));
            Guard.AgainstNull(_millionaire, () => missing(nameof(_millionaire)));
        }
    }
}

using System;
using System.Collections.Generic;
using ContentValidation;
using Core.Lifecycle;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.CharacterAnimation
{
    [DisallowMultipleComponent]
    public sealed class CharacterAnimationView
        : MonoBehaviour,
          ICharacterAnimationView,
          IValidatable,
          IWarmupLifecycle
    {
        [Serializable]
        private struct StateNamesMapItem
        {
            public CharacterAnimationSlot AnimationSlot;
            public string StateName;
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private float _crossFadeDuration = 0.1f;
        [SerializeField] private StateNamesMapItem[] _rawStatesMap;

        private Dictionary<CharacterAnimationSlot, int> _stateHashes;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_animator, () => Missing(nameof(_animator)));
            Guard.AgainstNullOrEmpty(_rawStatesMap, () => Missing(nameof(_rawStatesMap)));

            Guard.AgainstNegative(
                _crossFadeDuration,
                () => new InvalidCharacterAnimationViewValueException(
                    nameof(_crossFadeDuration),
                    gameObject.name,
                    _crossFadeDuration
                )
            );

            ValidateStateMap();

            return;

            ExtendedException Missing(string fieldName) =>
                new MissingCharacterAnimationViewFieldException(fieldName, gameObject.name);
        }

        void IWarmupLifecycle.Warmup()
        {
            _stateHashes = BuildStateHashes();
        }

        void ICharacterAnimationView.Play(CharacterAnimationSlot slot)
        {
            _animator.CrossFadeInFixedTime(_stateHashes[slot], _crossFadeDuration);
        }

        private void ValidateStateMap()
        {
            HashSet<CharacterAnimationSlot> mappedSlots = new HashSet<CharacterAnimationSlot>();

            foreach (StateNamesMapItem mapItem in _rawStatesMap)
            {
                Guard.AgainstTrue(
                    !mappedSlots.Add(mapItem.AnimationSlot),
                    () => new DuplicateCharacterAnimationSlotException(mapItem.AnimationSlot, gameObject.name)
                );

                Guard.AgainstTrue(
                    string.IsNullOrWhiteSpace(mapItem.StateName),
                    () => new CharacterAnimationStateNameMissingException(mapItem.AnimationSlot, gameObject.name)
                );
            }

            foreach (CharacterAnimationSlot slot in Enum.GetValues(typeof(CharacterAnimationSlot)))
            {
                Guard.AgainstTrue(
                    !mappedSlots.Contains(slot),
                    () => new CharacterAnimationSlotNotMappedException(slot, gameObject.name)
                );
            }
        }

        private Dictionary<CharacterAnimationSlot, int> BuildStateHashes()
        {
            Dictionary<CharacterAnimationSlot, int> stateHashes =
                new Dictionary<CharacterAnimationSlot, int>(_rawStatesMap.Length);

            foreach (StateNamesMapItem mapItem in _rawStatesMap)
            {
                stateHashes.Add(mapItem.AnimationSlot, Animator.StringToHash(mapItem.StateName));
            }

            return stateHashes;
        }
    }
}

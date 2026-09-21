using System;
using System.Collections.Generic;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.CharacterAnimation
{
    [DisallowMultipleComponent]
    public sealed class CharacterAnimationView
        : MonoBehaviour,
          ICharacterAnimationView
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

        private void Awake()
        {
            Validate();

            _stateHashes = BuildStateHashes();
        }

        public void Play(CharacterAnimationSlot slot)
        {
            Guard.AgainstTrue(
                !_stateHashes.TryGetValue(slot, out int stateHash),
                () => new CharacterAnimationSlotNotMappedException(slot, gameObject.name)
            );

            _animator.CrossFadeInFixedTime(stateHash, _crossFadeDuration);
        }

        private Dictionary<CharacterAnimationSlot, int> BuildStateHashes()
        {
            Dictionary<CharacterAnimationSlot, int> stateHashes =
                new Dictionary<CharacterAnimationSlot, int>(_rawStatesMap.Length);

            foreach (StateNamesMapItem mapItem in _rawStatesMap)
            {
                Guard.AgainstTrue(
                    stateHashes.ContainsKey(mapItem.AnimationSlot),
                    () => new DuplicateCharacterAnimationSlotException(mapItem.AnimationSlot, gameObject.name)
                );

                Guard.AgainstTrue(
                    string.IsNullOrWhiteSpace(mapItem.StateName),
                    () => new CharacterAnimationStateNameMissingException(mapItem.AnimationSlot, gameObject.name)
                );

                stateHashes.Add(mapItem.AnimationSlot, Animator.StringToHash(mapItem.StateName));
            }

            return stateHashes;
        }

        private void Validate()
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

            return;

            ExtendedException Missing(string fieldName) =>
                new MissingCharacterAnimationViewFieldException(fieldName, gameObject.name);
        }
    }
}

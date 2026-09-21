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
        private Dictionary<CharacterAnimationSlot, int> _reactionTriggers;

        private void Awake()
        {
            Validate();

            _stateHashes = BuildStateHashes();
            EnsureAllSlotsMapped(_stateHashes);
            _reactionTriggers = BuildReactionTriggers();
        }

        public void Play(CharacterAnimationSlot slot)
        {
            _animator.CrossFadeInFixedTime(_stateHashes[slot], _crossFadeDuration);
        }

        public void SetReaction(CharacterAnimationSlot slot)
        {
            foreach (int triggerHash in _reactionTriggers.Values)
            {
                _animator.ResetTrigger(triggerHash);
            }

            _animator.SetTrigger(_reactionTriggers[slot]);
        }

        private Dictionary<CharacterAnimationSlot, int> BuildStateHashes()
        {
            Dictionary<CharacterAnimationSlot, int> stateHashes =
                new Dictionary<CharacterAnimationSlot, int>(_rawStatesMap.Length);

            foreach (StateNamesMapItem mapItem in _rawStatesMap)
            {
                Guard.AgainstTrue(stateHashes.ContainsKey(mapItem.AnimationSlot), () =>
                    new DuplicateCharacterAnimationSlotException(mapItem.AnimationSlot, gameObject.name));

                Guard.AgainstTrue(string.IsNullOrWhiteSpace(mapItem.StateName), () =>
                    new CharacterAnimationStateNameMissingException(mapItem.AnimationSlot, gameObject.name));

                stateHashes.Add(mapItem.AnimationSlot, Animator.StringToHash(mapItem.StateName));
            }

            return stateHashes;
        }

        private void EnsureAllSlotsMapped(Dictionary<CharacterAnimationSlot, int> stateHashes)
        {
            foreach (CharacterAnimationSlot slot in Enum.GetValues(typeof(CharacterAnimationSlot)))
            {
                Guard.AgainstTrue(
                    !stateHashes.ContainsKey(slot),
                    () => new CharacterAnimationSlotNotMappedException(slot, gameObject.name)
                );
            }
        }

        private Dictionary<CharacterAnimationSlot, int> BuildReactionTriggers()
        {
            return new Dictionary<CharacterAnimationSlot, int>
            {
                [CharacterAnimationSlot.Happy] = _stateHashes[CharacterAnimationSlot.Happy],
                [CharacterAnimationSlot.Sad] = _stateHashes[CharacterAnimationSlot.Sad]
            };
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

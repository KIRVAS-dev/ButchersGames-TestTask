namespace ViewComponents.CharacterAnimation
{
    public interface ICharacterAnimationView
    {
        void Play(CharacterAnimationSlot slot);
        void SetReaction(CharacterAnimationSlot slot);
    }
}

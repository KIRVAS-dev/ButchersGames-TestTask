using Infrastructure.ExtendedExceptions;

namespace ViewComponents.AnimationSounds
{
    internal sealed class MissingAnimationSoundEmitterFieldException : ExtendedException
    {
        public MissingAnimationSoundEmitterFieldException(string fieldName, string objectName)
            : base("animation-sound-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class MissingAnimationSoundConfigException : ExtendedException
    {
        public MissingAnimationSoundConfigException(string clipName, string objectName)
            : base("animation-sound-2", $"PlaySound event in clip '{clipName}' has no AnimationSoundConfig on '{objectName}'") { }
    }

    internal sealed class MissingAnimationSoundException : ExtendedException
    {
        public MissingAnimationSoundException(string configName)
            : base("animation-sound-3", $"Missing FMOD event in animation sound config '{configName}'") { }
    }
}

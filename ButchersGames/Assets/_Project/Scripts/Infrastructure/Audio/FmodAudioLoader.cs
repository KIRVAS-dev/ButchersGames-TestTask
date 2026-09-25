using Core.Audio;
using Cysharp.Threading.Tasks;
using FMODUnity;
using System.Threading;

namespace Infrastructure.Audio
{
    public sealed class FmodAudioLoader : IAudioLoader
    {
        UniTask IAudioLoader.LoadAsync(CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(IsLoaded, cancellationToken: cancellationToken);
        }

        private bool IsLoaded()
        {
            return RuntimeManager.HaveAllBanksLoaded && !RuntimeManager.AnySampleDataLoading();
        }
    }
}

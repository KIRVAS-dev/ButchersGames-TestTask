using Cysharp.Threading.Tasks;
using Infrastructure.ExtendedExceptions;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoreLoadSceneMode = Core.Bootstrap.Scene.LoadSceneMode;
using UnityLoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace Core.Bootstrap.Scene
{
    public sealed class CoreLoader : ISceneLoader
    {
        public void LoadScene(string sceneName, CoreLoadSceneMode loadSceneMode)
        {
            UnityLoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            SceneManager.LoadScene(sceneName, unityLoadSceneMode);
        }

        public async UniTask LoadSceneAsync(
            string sceneName,
            CoreLoadSceneMode loadSceneMode,
            CancellationToken cancellationToken)
        {
            UnityLoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, unityLoadSceneMode);

            Guard.AgainstNull(loadOperation, () => new SceneNotFoundException(sceneName));

            await loadOperation.ToUniTask(cancellationToken: cancellationToken);
        }

        private UnityLoadSceneMode ConvertLoadSceneMode(CoreLoadSceneMode loadSceneMode)
        {
            return loadSceneMode switch
            {
                CoreLoadSceneMode.Single => UnityLoadSceneMode.Single,
                CoreLoadSceneMode.Additive => UnityLoadSceneMode.Additive,
                _ => throw new UnhandledLoadSceneModeException(loadSceneMode)
            };
        }
    }
}

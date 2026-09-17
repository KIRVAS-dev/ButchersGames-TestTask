using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoreLoadSceneMode = Core.Bootstrap.LoadSceneMode;
using UnityLoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace Core.Bootstrap
{
    public sealed class CoreLoader : ISceneLoader
    {
        public async UniTask LoadSceneAsync(string sceneName, CoreLoadSceneMode loadSceneMode,
            CancellationToken cancellationToken)
        {
            UnityLoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, unityLoadSceneMode);

            await loadOperation.ToUniTask(cancellationToken: cancellationToken);
        }

        private UnityLoadSceneMode ConvertLoadSceneMode(CoreLoadSceneMode loadSceneMode)
        {
            switch (loadSceneMode)
            {
                case CoreLoadSceneMode.Single:
                    return UnityLoadSceneMode.Single;

                case CoreLoadSceneMode.Additive:
                    return UnityLoadSceneMode.Additive;

                default:
                    throw new UnhandledLoadSceneModeException(loadSceneMode);
            }
        }
    }
}

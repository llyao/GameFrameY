using Cysharp.Threading.Tasks;
using GameFrameX.Runtime;
using GameFrameX.Scene.Runtime;
using UnityEngine.SceneManagement;
using YooAsset;

namespace GameFrameY.Bridge.Scene
{
    /// <summary>场景服务：包装 GameFrameX SceneComponent（底层 YooAsset）。</summary>
    public interface ISceneService
    {
        UniTask<SceneHandle> LoadSceneAsync(string sceneAssetName, LoadSceneMode mode = LoadSceneMode.Single);

        void UnloadScene(string sceneAssetName);

        Camera MainCamera { get; }
    }

    public sealed class SceneService : ISceneService
    {
        private static SceneComponent Component => GameEntry.GetComponent<SceneComponent>();

        public async UniTask<SceneHandle> LoadSceneAsync(string sceneAssetName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = await Component.LoadScene(sceneAssetName, mode);
            return handle;
        }

        public void UnloadScene(string sceneAssetName)
        {
            Component.UnloadScene(sceneAssetName);
        }

        public Camera MainCamera => Component.MainCamera;
    }
}

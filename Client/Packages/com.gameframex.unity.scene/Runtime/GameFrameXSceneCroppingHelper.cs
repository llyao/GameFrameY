using UnityEngine;

namespace GameFrameX.Scene.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public class GameFrameXSceneCroppingHelper : MonoBehaviour
    {
        [UnityEngine.Scripting.Preserve]
        private void Start()
        {
            _ = typeof(SceneComponent);
            _ = typeof(ActiveSceneChangedEventArgs);
            _ = typeof(GameSceneManager);
            _ = typeof(IGameSceneManager);
            _ = typeof(LoadSceneFailureEventArgs);
            _ = typeof(LoadSceneSuccessEventArgs);
            _ = typeof(LoadSceneUpdateEventArgs);
            _ = typeof(UnloadSceneFailureEventArgs);
            _ = typeof(UnloadSceneSuccessEventArgs);
        }
    }
}
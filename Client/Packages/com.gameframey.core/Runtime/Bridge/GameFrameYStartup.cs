using GameFrameX.Asset.Runtime;
using GameFrameX.Procedure.Runtime;
using GameFrameX.Scene.Runtime;
using UnityEngine;
using VContainer.Unity;
using YooAsset;

namespace GameFrameY.Bridge
{
    /// <summary>
    /// GameFrameY 组合根：在 BeforeSceneLoad 配置 GameFrameX 组件参数并创建 VContainer 根作用域。
    /// 不需要场景中摆放任何东西（GameFrameX 组件也是零配置自举的）。
    /// </summary>
    public static class GameFrameYStartup
    {
        private static bool _rootCreated;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            // 关闭 Domain Reload 时清空静态残留。
            _rootCreated = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (_rootCreated)
            {
                return;
            }

            _rootCreated = true;

            ConfigureGameFrameXOptions();

            var scope = LifetimeScope.Create(new GameFrameYRootInstaller(), "GameFrameY Root");
            if (scope != null)
            {
                Object.DontDestroyOnLoad(scope.gameObject);
            }
        }

        /// <summary>
        /// GameFrameXOptions 必须在组件首次消费前（BeforeSceneLoad）注入；
        /// 手动 Configure 优先于任何 IGameFrameXOptionsContributor。
        /// 资源 PlayMode 默认 EditorSimulateMode，后续可按渠道切换 Offline/Host。
        /// </summary>
        private static void ConfigureGameFrameXOptions()
        {
            GameFrameX.Runtime.GameFrameXOptions.Configure(new AssetOptions
            {
                GamePlayMode = EPlayMode.EditorSimulateMode,
            });
            GameFrameX.Runtime.GameFrameXOptions.Configure(new SceneOptions());
            GameFrameX.Runtime.GameFrameXOptions.Configure(new ProcedureOptions
            {
                // 流程由 GameFrameYGameLauncher 自管（ReinitializeProcedures + StartProcedure）。
                UseStartupRunner = true,
            });
        }
    }
}

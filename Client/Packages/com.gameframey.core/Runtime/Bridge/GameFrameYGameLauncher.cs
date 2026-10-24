using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFrameX.Asset.Runtime;
using GameFrameX.Procedure.Runtime;
using GameFrameX.Runtime;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;
using VContainer.Unity;

namespace GameFrameY.Bridge
{
    /// <summary>
    /// 启动协调器（VContainer IAsyncStartable，首帧 Startup 时机执行）：
    /// 等 GameFrameX 组件宿主就绪 → 装配 Bootstrap 阶段模块 → 组装并启动默认流程链
    /// Launch → Patch → LoadConfig → EnterGame。
    /// </summary>
    public sealed class GameFrameYGameLauncher : IAsyncStartable
    {
        private readonly GameModuleManager _modules;
        private readonly IServiceRegistry _services;
        private readonly GameFrameYLaunchSettings _settings;

        public GameFrameYGameLauncher(GameModuleManager modules, IServiceRegistry services, GameFrameYLaunchSettings settings)
        {
            _modules = modules;
            _services = services;
            _settings = settings;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            // GameFrameX 组件宿主在 AfterSceneLoad 创建；IAsyncStartable 早于其执行时需要等待。
            await UniTask.WaitUntil(IsGameFrameXReady, cancellationToken: cancellation);

            GfyLog.Info("GameFrameX 组件就绪，开始装配 Bootstrap 模块...");
            await _modules.InitializeBootstrapModulesAsync();

            StartDefaultProcedures();

            GfyLog.Info("启动流程已开始（Launch → Patch → LoadConfig → EnterGame）");
        }

        private bool IsGameFrameXReady()
        {
            try
            {
                return GameEntry.GetComponent<ProcedureComponent>() != null
                       && GameEntry.GetComponent<AssetComponent>() != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void StartDefaultProcedures()
        {
            var procedureComponent = GameEntry.GetComponent<ProcedureComponent>();
            var launch = new ProcedureLaunch(_modules, _services, _settings);
            var patch = new ProcedurePatch(_modules, _services, _settings);
            var loadConfig = new ProcedureLoadConfig(_modules, _services, _settings);
            var enterGame = new ProcedureEnterGame(_modules, _services, _settings);

            var procedures = new GameFrameX.Procedure.Runtime.ProcedureBase[] { launch, patch, loadConfig, enterGame };
            procedureComponent.ReinitializeProcedures(procedures, launch);

            if (procedureComponent.CurrentProcedure == null)
            {
                procedureComponent.Procedure.StartProcedure(launch.GetType());
            }
        }
    }
}

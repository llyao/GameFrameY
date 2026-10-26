using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFrameX.Asset.Runtime;
using GameFrameX.Procedure.Runtime;
using GameFrameX.Fsm.Runtime;
using GameFrameX.Runtime;
using GameFrameY.Bridge.Procedures;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;
using GameFrameY.Core.Startup;
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
        private readonly GameFrameYStartupStatus _startupStatus;

        public GameFrameYGameLauncher(
            GameModuleManager modules,
            IServiceRegistry services,
            GameFrameYLaunchSettings settings,
            GameFrameYStartupStatus startupStatus)
        {
            _modules = modules;
            _services = services;
            _settings = settings;
            _startupStatus = startupStatus;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            try
            {
                _startupStatus.TryTransition(GameFrameYStartupState.Bootstrapping);
                await UniTask.WaitUntil(IsGameFrameXReady, cancellationToken: cancellation);

                GfyLog.Info("GameFrameX 组件就绪，开始装配 Bootstrap 模块...");
                await _modules.InitializeBootstrapModulesAsync();
                _startupStatus.TryTransition(GameFrameYStartupState.Patching);

                StartDefaultProcedures();

                GfyLog.Info("启动流程已开始（Launch → Patch → LoadConfig → EnterGame）");
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                _startupStatus.TryTransition(GameFrameYStartupState.Stopping);
                throw;
            }
            catch (Exception e)
            {
                _startupStatus.TryFail(e);
                GfyLog.Error($"GameFrameY 启动失败: {e}");
                throw;
            }
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
            var launch = new ProcedureLaunch(_modules, _services, _settings, _startupStatus);
            var patch = new ProcedurePatch(_modules, _services, _settings, _startupStatus);
            var loadConfig = new ProcedureLoadConfig(_modules, _services, _settings, _startupStatus);
            var enterGame = new ProcedureEnterGame(_modules, _services, _settings, _startupStatus);

            var procedures = new GameFrameX.Procedure.Runtime.ProcedureBase[] { launch, patch, loadConfig, enterGame };
            // ProcedureOptions.UseStartupRunner=true 时，GameFrameX 只注册 IProcedureManager，
            // 不会替我们调用 Initialize；先显式创建其内部 FSM，再注册入口流程。
            procedureComponent.Procedure.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures);
            procedureComponent.ReinitializeProcedures(procedures, launch);

            procedureComponent.Procedure.StartProcedure(launch.GetType());
        }
    }
}

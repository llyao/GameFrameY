using Cysharp.Threading.Tasks;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;

namespace GameFrameY.Bridge.Procedures
{
    /// <summary>
    /// 资源补丁流程：初始化 YooAsset 资源包（EditorSimulateMode 下为模拟构建初始化）。
    /// HostPlayMode 的静态版本/清单/下载完整流后续在 IAssetService 内扩展。
    /// </summary>
    public sealed class ProcedurePatch : GameFrameYProcedure
    {
        public ProcedurePatch(GameModuleManager modules, IServiceRegistry services, GameFrameYLaunchSettings settings)
            : base(modules, services, settings)
        {
        }

        protected override void OnEnter(GameFrameX.Fsm.Runtime.IFsm<GameFrameX.Procedure.Runtime.IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            RunFlow(PatchFlowAsync);
        }

        private async UniTask PatchFlowAsync()
        {
            // 等 GameFrameX 组件自身 Start() 完成初始化（两帧防御）。
            await UniTask.DelayFrame(2);

            var asset = Services.GetService<IAssetService>();
            var ok = await asset.InitializeAsync(Settings.PlayMode, Settings.HostUrl, Settings.FallbackHostUrl);
            if (!ok)
            {
                GfyLog.Error($"资源包初始化失败（PlayMode={Settings.PlayMode}），停留在 Patch 流程等待处理");
                return;
            }

            GfyLog.Info("资源包初始化完成");
            GoTo<ProcedureLoadConfig>();
        }
    }
}

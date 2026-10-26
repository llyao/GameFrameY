using Cysharp.Threading.Tasks;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;
using GameFrameY.Core.Startup;

namespace GameFrameY.Bridge.Procedures
{
    /// <summary>配置加载流程：经 YooAsset 读取 Luban 生成的 bytes 并构造 cfg.Tables。</summary>
    public sealed class ProcedureLoadConfig : GameFrameYProcedure
    {
        public ProcedureLoadConfig(
            GameModuleManager modules,
            IServiceRegistry services,
            GameFrameYLaunchSettings settings,
            GameFrameYStartupStatus startupStatus)
            : base(modules, services, settings, startupStatus)
        {
        }

        protected override void OnEnter(GameFrameX.Fsm.Runtime.IFsm<GameFrameX.Procedure.Runtime.IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            RunFlow(LoadConfigFlowAsync);
        }

        private async UniTask LoadConfigFlowAsync()
        {
            StartupStatus.TryTransition(GameFrameYStartupState.LoadingConfig);
            var config = Services.GetService<IConfigService>();
            await config.LoadAsync();
            GfyLog.Info($"Luban 配置加载完成：TbItem {config.Tables.TbItem.DataList.Count} 条，" +
                        $"TbServerList {config.Tables.TbServerList.DataList.Count} 条");
            GoTo<ProcedureEnterGame>();
        }
    }
}

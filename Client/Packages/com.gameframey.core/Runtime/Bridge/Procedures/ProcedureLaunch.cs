using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;

namespace GameFrameY.Bridge.Procedures
{
    /// <summary>启动横幅与前置检查，随后进入补丁流程。</summary>
    public sealed class ProcedureLaunch : GameFrameYProcedure
    {
        public ProcedureLaunch(GameModuleManager modules, IServiceRegistry services, GameFrameYLaunchSettings settings)
            : base(modules, services, settings)
        {
        }

        protected override void OnEnter(GameFrameX.Fsm.Runtime.IFsm<GameFrameX.Procedure.Runtime.IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GfyLog.Info("GameFrameY 启动（VContainer 模块体系 + GameFrameX 组件 + YooAsset + Luban）");
            GoTo<ProcedurePatch>();
        }
    }
}

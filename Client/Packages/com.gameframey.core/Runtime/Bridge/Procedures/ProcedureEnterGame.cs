using Cysharp.Threading.Tasks;
using GameFrameX.Event.Runtime;
using GameFrameX.Runtime;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;

namespace GameFrameY.Bridge.Procedures
{
    /// <summary>进入游戏：装配 Deferred 阶段模块（业务模块），广播 GameReady。</summary>
    public sealed class ProcedureEnterGame : GameFrameYProcedure
    {
        public ProcedureEnterGame(GameModuleManager modules, IServiceRegistry services, GameFrameYLaunchSettings settings)
            : base(modules, services, settings)
        {
        }

        protected override void OnEnter(GameFrameX.Fsm.Runtime.IFsm<GameFrameX.Procedure.Runtime.IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            RunFlow(EnterGameFlowAsync);
        }

        private async UniTask EnterGameFlowAsync()
        {
            await Modules.InstallDeferredModulesAsync();

            GameEntry.GetComponent<EventComponent>()?.Fire(this, GameFrameYEventIds.GameReady);
            GfyLog.Info("GameFrameY 启动流程完成（GameReady）");
        }
    }
}

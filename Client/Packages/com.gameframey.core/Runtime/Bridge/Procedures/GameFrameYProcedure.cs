using System;
using Cysharp.Threading.Tasks;
using GameFrameX.Fsm.Runtime;
using GameFrameX.Procedure.Runtime;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using GameFrameY.Core.Service;
using GameFrameY.Core.Startup;
using UnityEngine;

namespace GameFrameY.Bridge.Procedures
{
    /// <summary>GameFrameY 流程基类：携带模块管理器/服务注册表/启动配置，提供异步流程与跳转助手。</summary>
    public abstract class GameFrameYProcedure : ProcedureBase
    {
        protected GameFrameYProcedure(
            GameModuleManager modules,
            IServiceRegistry services,
            GameFrameYLaunchSettings settings,
            GameFrameYStartupStatus startupStatus)
        {
            Modules = modules;
            Services = services;
            Settings = settings;
            StartupStatus = startupStatus;
        }

        protected GameModuleManager Modules { get; }
        protected IServiceRegistry Services { get; }
        protected GameFrameYLaunchSettings Settings { get; }
        protected GameFrameYStartupStatus StartupStatus { get; }

        protected IFsm<IProcedureManager> Fsm { get; private set; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Fsm = procedureOwner;
        }

        protected void GoTo<TState>() where TState : ProcedureBase
        {
            ChangeState<TState>(Fsm);
        }

        /// <summary>OnEnter 等同步回调里启动异步流程；异常进入 Failed 状态并记录。</summary>
        protected void RunFlow(Func<UniTask> flow)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    await flow();
                }
                catch (Exception e)
                {
                    StartupStatus.TryFail(e);
                    GfyLog.Error($"流程 {GetType().Name} 异步流程异常: {e}");
                }
            });
        }
    }
}

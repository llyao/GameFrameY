using Cysharp.Threading.Tasks;
using GameFrameY.Core.Service;
using VContainer;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 模块可选基类：提供属性注入的 <see cref="Services"/> 与便捷的服务登记。
    /// 派生类可自由声明构造函数参数（由所在子作用域及其祖先作用域解析）。
    /// </summary>
    public abstract class GameModuleBase : IGameModule
    {
        public GameModuleDescriptor Descriptor { get; set; }

        [Inject]
        public IServiceRegistry Services { get; private set; }

        public virtual UniTask OnInitializeAsync() => UniTask.CompletedTask;

        public virtual UniTask OnStartAsync() => UniTask.CompletedTask;

        public virtual UniTask OnShutdownAsync() => UniTask.CompletedTask;

        /// <summary>
        /// 向中央服务注册表登记一个服务实例（登记为 <typeparamref name="TService"/> 类型，
        /// 归属当前模块）。模块卸载时自动注销。用于无法走构造注入的消费方（热更代码、场景对象等）。
        /// </summary>
        protected void RegisterService<TService>(TService instance) where TService : class
        {
            Services?.Register(instance, Descriptor?.Name);
        }
    }
}

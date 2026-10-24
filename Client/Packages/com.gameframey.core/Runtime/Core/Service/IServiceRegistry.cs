using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFrameY.Core.Service
{
    /// <summary>
    /// 服务发现入口（两级查找）：
    /// 1. 中央注册表 —— 模块运行期用 <c>RegisterService</c> 登记的实例（含归属模块、增删事件）；
    /// 2. 根容器兜底 —— 各模块 Installer 注册的类型服务（VContainer TryResolve）。
    /// 构造注入仍然是首选的消费方式；本接口面向无法注入的场景（热更代码、场景对象、调试）。
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>服务登记/注销通知（服务发现的动态发现语义）。</summary>
        event Action<ServiceChangeEventArgs> ServiceChanged;

        /// <summary>登记一个服务实例（通常由 GameModuleBase.RegisterService 调用）。</summary>
        void Register<TService>(TService instance, string ownerModuleId = null) where TService : class;

        void Register(Type serviceType, object instance, string ownerModuleId = null);

        bool TryGetService<TService>(out TService service) where TService : class;

        TService GetService<TService>() where TService : class;

        /// <summary>同一服务类型的全部实现（多实现聚合）。</summary>
        IReadOnlyList<TService> GetServices<TService>() where TService : class;

        IReadOnlyList<object> GetAllServices(Type serviceType);

        /// <summary>注销指定实例（登记时才可见；容器类型注册不受影响）。</summary>
        bool Unregister<TService>(TService instance) where TService : class;

        /// <summary>注销某模块登记的全部服务（模块卸载时由模块管理器调用）。</summary>
        int UnregisterAll(string ownerModuleId);

        /// <summary>服务尚未登记时挂起等待；登记或已可解析时立即返回。</summary>
        UniTask<TService> WaitServiceAsync<TService>(CancellationToken cancellationToken = default) where TService : class;
    }
}

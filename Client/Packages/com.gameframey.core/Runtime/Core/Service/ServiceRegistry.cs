using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace GameFrameY.Core.Service
{
    /// <summary>
    /// 默认实现：中央登记表（按类型聚合，后登记优先）+ 根作用域 TryResolve 兜底。
    /// 模块子作用域的 Installer 服务不会被根 resolver 向下查找，需通过模块显式导出进入中央表。
    /// 仅设计为主线程使用（与 VContainer 解析、Unity 生命周期一致）。
    /// </summary>
    public sealed class ServiceRegistry : IServiceRegistry
    {
        private readonly IObjectResolver _rootResolver;
        private readonly Dictionary<Type, List<ServiceDescriptor>> _registered = new();

        public ServiceRegistry(IObjectResolver rootResolver)
        {
            _rootResolver = rootResolver;
        }

        public event Action<ServiceChangeEventArgs> ServiceChanged;

        public void Register<TService>(TService instance, string ownerModuleId = null) where TService : class
        {
            Register(typeof(TService), instance, ownerModuleId);
        }

        public void Register(Type serviceType, object instance, string ownerModuleId = null)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!serviceType.IsInstanceOfType(instance))
            {
                throw new ServiceRegistryException($"实例 {instance.GetType().FullName} 不是服务类型 {serviceType.FullName} 的实现");
            }

            if (!_registered.TryGetValue(serviceType, out var list))
            {
                list = new List<ServiceDescriptor>();
                _registered[serviceType] = list;
            }

            if (list.Any(d => ReferenceEquals(d.Instance, instance)))
            {
                return;
            }

            var descriptor = new ServiceDescriptor(serviceType, instance, ownerModuleId);
            list.Add(descriptor);
            ServiceChanged?.Invoke(new ServiceChangeEventArgs(ServiceChangeKind.Added, descriptor));
        }

        public bool TryGetService<TService>(out TService service) where TService : class
        {
            if (TryGetFromRegistry(typeof(TService), out var instance))
            {
                service = (TService)instance;
                return true;
            }

            // 容器兜底：根作用域直接注册的类型服务；模块子作用域服务需显式导出到中央表。
            if (_rootResolver.TryResolve<TService>(out service))
            {
                return true;
            }

            service = null;
            return false;
        }

        public TService GetService<TService>() where TService : class
        {
            if (TryGetService<TService>(out var service))
            {
                return service;
            }

            throw new ServiceRegistryException($"服务未注册: {typeof(TService).FullName}");
        }

        public IReadOnlyList<TService> GetServices<TService>() where TService : class
        {
            var result = new List<TService>();
            if (_registered.TryGetValue(typeof(TService), out var list))
            {
                result.AddRange(list.Select(d => (TService)d.Instance));
            }

            if (_rootResolver.TryResolve<IEnumerable<TService>>(out var fromContainer))
            {
                var known = new HashSet<object>(result.Select(s => (object)s));
                result.AddRange(fromContainer.Where(s => !known.Contains(s)));
            }

            return result;
        }

        public IReadOnlyList<object> GetAllServices(Type serviceType)
        {
            var result = new List<object>();
            if (_registered.TryGetValue(serviceType, out var list))
            {
                result.AddRange(list.Select(d => d.Instance));
            }

            return result;
        }

        public bool Unregister<TService>(TService instance) where TService : class
        {
            if (instance == null || !_registered.TryGetValue(typeof(TService), out var list))
            {
                return false;
            }

            var index = list.FindIndex(d => ReferenceEquals(d.Instance, instance));
            if (index < 0)
            {
                return false;
            }

            var descriptor = list[index];
            list.RemoveAt(index);
            if (list.Count == 0)
            {
                _registered.Remove(typeof(TService));
            }

            ServiceChanged?.Invoke(new ServiceChangeEventArgs(ServiceChangeKind.Removed, descriptor));
            return true;
        }

        public int UnregisterAll(string ownerModuleId)
        {
            if (string.IsNullOrEmpty(ownerModuleId))
            {
                return 0;
            }

            var removedDescriptors = new List<ServiceDescriptor>();
            foreach (var key in _registered.Keys.ToList())
            {
                var list = _registered[key];
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (!string.Equals(list[i].OwnerModuleId, ownerModuleId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    removedDescriptors.Add(list[i]);
                    list.RemoveAt(i);
                }

                if (list.Count == 0)
                {
                    _registered.Remove(key);
                }
            }

            foreach (var descriptor in removedDescriptors)
            {
                ServiceChanged?.Invoke(new ServiceChangeEventArgs(ServiceChangeKind.Removed, descriptor));
            }

            return removedDescriptors.Count;
        }

        public async UniTask<TService> WaitServiceAsync<TService>(CancellationToken cancellationToken = default) where TService : class
        {
            if (TryGetService<TService>(out var service))
            {
                return service;
            }

            var completion = new UniTaskCompletionSource<TService>();
            void OnChanged(ServiceChangeEventArgs e)
            {
                if (e.Kind == ServiceChangeKind.Added && e.Descriptor.ServiceType == typeof(TService))
                {
                    ServiceChanged -= OnChanged;
                    completion.TrySetResult(GetService<TService>());
                }
            }

            ServiceChanged += OnChanged;

            // 订阅后二次检查，避免登记与订阅之间的竞态。
            if (TryGetService<TService>(out service))
            {
                ServiceChanged -= OnChanged;
                return service;
            }

            var cancellationRegistration = cancellationToken.Register(() =>
            {
                ServiceChanged -= OnChanged;
                completion.TrySetCanceled();
            });

            try
            {
                return await completion.Task;
            }
            finally
            {
                cancellationRegistration.Dispose();
            }
        }

        private bool TryGetFromRegistry(Type serviceType, out object instance)
        {
            instance = null;
            if (!_registered.TryGetValue(serviceType, out var list) || list.Count == 0)
            {
                return false;
            }

            // 后登记者优先（便于覆盖）。
            instance = list[list.Count - 1].Instance;
            return true;
        }
    }
}

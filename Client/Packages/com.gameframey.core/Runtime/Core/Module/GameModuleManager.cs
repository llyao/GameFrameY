using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFrameY.Core.Service;
using GameFrameY.Core.Startup;
using VContainer;
using VContainer.Unity;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 模块管理器：装配 = 按依赖拓扑序为每个模块创建 VContainer 子作用域（当前实现为线性父链，
    /// 依赖声明用于排序和依赖存在性校验；需要跨模块定位的服务必须显式导出到中央注册表）。
    /// 生命周期（Initialize/Start/Tick/Shutdown）由本管理器统一驱动；Tick 泵本身以 VContainer EntryPoint 挂在 PlayerLoop 上。
    /// </summary>
    public sealed class GameModuleManager : ITickable, IFixedTickable, ILateTickable
    {
        private readonly LifetimeScope _rootScope;
        private readonly IServiceRegistry _serviceRegistry;
        private readonly GameFrameYStartupStatus _startupStatus;
        private readonly List<GameModuleDescriptor> _installed = new();
        private readonly Dictionary<string, GameModuleDescriptor> _installedByName = new(StringComparer.OrdinalIgnoreCase);
        private LifetimeScope _lastCreatedScope;
        private bool _bootstrapInstalled;

        public GameModuleManager(
            LifetimeScope rootScope,
            IServiceRegistry serviceRegistry,
            GameFrameYStartupStatus startupStatus)
        {
            _rootScope = rootScope ?? throw new ArgumentNullException(nameof(rootScope));
            _serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
            _startupStatus = startupStatus ?? throw new ArgumentNullException(nameof(startupStatus));
        }

        /// <summary>已装配模块（装配顺序）。</summary>
        public IReadOnlyList<GameModuleDescriptor> InstalledModules => _installed;

        public bool TryGetModule(string name, out GameModuleDescriptor descriptor)
        {
            return _installedByName.TryGetValue(name, out descriptor);
        }

        /// <summary>
        /// 装配全部 Bootstrap 阶段模块。在根作用域与 GameFrameX 组件就绪后调用一次。
        /// </summary>
        public async UniTask InitializeBootstrapModulesAsync(params Assembly[] assemblies)
        {
            if (_bootstrapInstalled)
            {
                return;
            }

            var descriptors = ResolveDescriptors(assemblies)
                .Where(d => d.Startup == ModuleStartup.Bootstrap)
                .ToList();
            await InstallCoreAsync(descriptors);
            _bootstrapInstalled = true;
        }

        /// <summary>
        /// 装配 Deferred 阶段模块（如业务模块，在资源补丁/配置加载完成后调用）。
        /// 传入程序集列表可只装配指定来源（热更程序集加载后二次装配）。
        /// </summary>
        public async UniTask InstallDeferredModulesAsync(params Assembly[] assemblies)
        {
            var descriptors = ResolveDescriptors(assemblies)
                .Where(d => d.Startup == ModuleStartup.Deferred && !_installedByName.ContainsKey(d.Name))
                .ToList();
            await InstallCoreAsync(descriptors);
        }

        /// <summary>按装配逆序卸载全部模块并注销其登记的服务。</summary>
        public async UniTask ShutdownAsync()
        {
            _startupStatus.TryTransition(GameFrameYStartupState.Stopping);
            for (var i = _installed.Count - 1; i >= 0; i--)
            {
                var descriptor = _installed[i];
                try
                {
                    if (descriptor.ModuleInstance != null &&
                        (descriptor.State is GameModuleState.Installing or GameModuleState.Initialized or GameModuleState.Started))
                    {
                        await descriptor.ModuleInstance.OnShutdownAsync();
                    }
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} OnShutdownAsync 异常: {e}");
                }

                try
                {
                    _serviceRegistry.UnregisterAll(descriptor.Name);
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} 注销服务异常: {e}");
                }

                try
                {
                    descriptor.Scope?.Dispose();
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} 释放作用域异常: {e}");
                }

                descriptor.State = GameModuleState.Shutdown;
                descriptor.Scope = null;
                descriptor.ModuleInstance = null;
            }

            _installed.Clear();
            _installedByName.Clear();
            _lastCreatedScope = null;
            _bootstrapInstalled = false;
            _startupStatus.TryTransition(GameFrameYStartupState.Stopped);
        }

        void ITickable.Tick() => PumpModules<ITickableModule>(static (m, dt, rdt) => m.OnTick(dt, rdt), UnityEngine.Time.deltaTime, UnityEngine.Time.unscaledDeltaTime);

        void IFixedTickable.FixedTick() => PumpModules<IFixedTickableModule>(static (m, dt, rdt) => m.OnFixedTick(dt, rdt), UnityEngine.Time.fixedDeltaTime, UnityEngine.Time.unscaledDeltaTime);

        void ILateTickable.LateTick() => PumpModules<ILateTickableModule>(static (m, dt, rdt) => m.OnLateTick(dt, rdt), UnityEngine.Time.deltaTime, UnityEngine.Time.unscaledDeltaTime);

        private void PumpModules<TInterface>(Action<TInterface, float, float> pump, float dt, float realDt)
        {
            // 本方法由 VContainer PlayerLoop 入口在主线程调用；_installed 仅在主线程装配/卸载时变更。
            for (var i = 0; i < _installed.Count; i++)
            {
                var descriptor = _installed[i];
                if (descriptor.State != GameModuleState.Started || descriptor.ModuleInstance is not TInterface tickable)
                {
                    continue;
                }

                try
                {
                    pump(tickable, dt, realDt);
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} Tick 异常: {e}");
                }
            }
        }

        private IReadOnlyList<GameModuleDescriptor> ResolveDescriptors(Assembly[] assemblies)
        {
            var types = ModuleScanner.ScanModules(assemblies);
            var descriptors = ModuleDependencyResolver.Resolve(types, _installedByName.Keys);
            foreach (var descriptor in descriptors)
            {
                if (_installedByName.TryGetValue(descriptor.Name, out var installed) &&
                    installed.ModuleType != descriptor.ModuleType)
                {
                    throw new GameModuleException(
                        $"模块名重复: {descriptor.Name} 已安装类型 {installed.ModuleType.FullName}，" +
                        $"新扫描类型为 {descriptor.ModuleType.FullName}");
                }
            }

            return descriptors;
        }

        private async UniTask InstallCoreAsync(IReadOnlyList<GameModuleDescriptor> descriptors)
        {
            var batch = new List<GameModuleDescriptor>();
            var previousLastScope = _lastCreatedScope;
            var batchLastScope = previousLastScope;

            try
            {
                foreach (var descriptor in descriptors)
                {
                    if (_installedByName.ContainsKey(descriptor.Name))
                    {
                        continue;
                    }

                    var installer = CreateInstaller(descriptor);
                    var parent = batchLastScope ?? _rootScope;
                    var scope = parent.CreateChild(installer, $"GameFrameY.Module.{descriptor.Name}");
                    descriptor.Scope = scope;
                    descriptor.State = GameModuleState.Installing;
                    batch.Add(descriptor);

                    var module = (IGameModule)scope.Container.Resolve(descriptor.ModuleType);
                    module.Descriptor = descriptor;
                    descriptor.ModuleInstance = module;
                    batchLastScope = scope;

                    await module.OnInitializeAsync();
                    descriptor.State = GameModuleState.Initialized;
                    _installed.Add(descriptor);
                    _installedByName[descriptor.Name] = descriptor;
                }

                foreach (var descriptor in batch)
                {
                    await descriptor.ModuleInstance.OnStartAsync();
                    descriptor.State = GameModuleState.Started;
                    GfyLog.Info($"模块就绪: {descriptor.Name} ({descriptor.ModuleType.Name})");
                }

                _lastCreatedScope = batchLastScope;
            }
            catch
            {
                await RollbackBatchAsync(batch);
                foreach (var descriptor in batch)
                {
                    _installed.Remove(descriptor);
                    _installedByName.Remove(descriptor.Name);
                }

                _lastCreatedScope = previousLastScope;
                throw;
            }
        }

        private async UniTask RollbackBatchAsync(IReadOnlyList<GameModuleDescriptor> batch)
        {
            for (var i = batch.Count - 1; i >= 0; i--)
            {
                var descriptor = batch[i];
                try
                {
                    if (descriptor.ModuleInstance != null &&
                        (descriptor.State is GameModuleState.Installing or GameModuleState.Initialized or GameModuleState.Started))
                    {
                        await descriptor.ModuleInstance.OnShutdownAsync();
                    }
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} 回滚时 OnShutdownAsync 异常: {e}");
                }

                try
                {
                    _serviceRegistry.UnregisterAll(descriptor.Name);
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} 回滚时注销服务异常: {e}");
                }

                try
                {
                    descriptor.Scope?.Dispose();
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} 回滚时释放作用域异常: {e}");
                }

                descriptor.State = GameModuleState.Shutdown;
                descriptor.Scope = null;
                descriptor.ModuleInstance = null;
            }
        }

        private IInstaller CreateInstaller(GameModuleDescriptor descriptor)
        {
            IInstaller inner = null;
            if (descriptor.InstallerType != null)
            {
                if (!typeof(IInstaller).IsAssignableFrom(descriptor.InstallerType))
                {
                    throw new GameModuleException(
                        $"模块 {descriptor.Name} 的 InstallerType {descriptor.InstallerType.FullName} 未实现 VContainer.IInstaller");
                }

                inner = (IInstaller)Activator.CreateInstance(descriptor.InstallerType);
            }

            return new ModuleScopeInstaller(descriptor.ModuleType, inner);
        }

        /// <summary>把模块类型注册进子作用域，再执行模块自带 Installer。</summary>
        private sealed class ModuleScopeInstaller : IInstaller
        {
            private readonly Type _moduleType;
            private readonly IInstaller _inner;

            public ModuleScopeInstaller(Type moduleType, IInstaller inner)
            {
                _moduleType = moduleType;
                _inner = inner;
            }

            public void Install(IContainerBuilder builder)
            {
                builder.Register(_moduleType, _moduleType, Lifetime.Singleton);
                _inner?.Install(builder);
            }
        }
    }
}

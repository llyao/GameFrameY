using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFrameY.Core.Service;
using VContainer;
using VContainer.Unity;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 模块管理器：装配 = 按依赖拓扑序为每个模块创建 VContainer 子作用域（链式父子：
    /// 后装模块的作用域以先装模块为父，因此模块只能构造注入"自己声明的依赖 + 根作用域"提供的服务，
    /// 未声明的跨模块引用在装配期即失败）。生命周期（Initialize/Start/Tick/Shutdown）由本管理器统一驱动；
    /// Tick 泵本身以 VContainer EntryPoint 挂在 PlayerLoop 上。
    /// </summary>
    public sealed class GameModuleManager : ITickable, IFixedTickable, ILateTickable
    {
        private readonly LifetimeScope _rootScope;
        private readonly IServiceRegistry _serviceRegistry;
        private readonly List<GameModuleDescriptor> _installed = new();
        private readonly Dictionary<string, GameModuleDescriptor> _installedByName = new(StringComparer.OrdinalIgnoreCase);
        private LifetimeScope _lastCreatedScope;
        private bool _bootstrapInstalled;

        public GameModuleManager(LifetimeScope rootScope, IServiceRegistry serviceRegistry)
        {
            _rootScope = rootScope ?? throw new ArgumentNullException(nameof(rootScope));
            _serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
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

            _bootstrapInstalled = true;
            var descriptors = ResolveDescriptors(assemblies)
                .Where(d => d.Startup == ModuleStartup.Bootstrap)
                .ToList();
            await InstallCoreAsync(descriptors);
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
            for (var i = _installed.Count - 1; i >= 0; i--)
            {
                var descriptor = _installed[i];
                try
                {
                    if (descriptor.State is GameModuleState.Initialized or GameModuleState.Started)
                    {
                        await descriptor.ModuleInstance.OnShutdownAsync();
                    }
                }
                catch (Exception e)
                {
                    GfyLog.Error($"模块 {descriptor.Name} OnShutdownAsync 异常: {e}");
                }

                _serviceRegistry.UnregisterAll(descriptor.Name);
                descriptor.Scope?.Dispose();
                descriptor.State = GameModuleState.Shutdown;
            }

            _installed.Clear();
            _installedByName.Clear();
            _lastCreatedScope = null;
            _bootstrapInstalled = false;
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
            return ModuleDependencyResolver.Resolve(types);
        }

        private async UniTask InstallCoreAsync(IReadOnlyList<GameModuleDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (_installedByName.ContainsKey(descriptor.Name))
                {
                    continue;
                }

                var installer = CreateInstaller(descriptor);
                var parent = _lastCreatedScope ?? _rootScope;
                var scope = parent.CreateChild(installer, $"GameFrameY.Module.{descriptor.Name}");
                descriptor.Scope = scope;
                descriptor.State = GameModuleState.Installing;

                var module = (IGameModule)scope.Container.Resolve(descriptor.ModuleType);
                module.Descriptor = descriptor;
                descriptor.ModuleInstance = module;

                _installed.Add(descriptor);
                _installedByName[descriptor.Name] = descriptor;
                _lastCreatedScope = scope;

                await module.OnInitializeAsync();
                descriptor.State = GameModuleState.Initialized;
            }

            // Start 阶段：同一批内按装配顺序串行启动。
            foreach (var descriptor in _installed.Where(d => d.State == GameModuleState.Initialized).ToList())
            {
                await descriptor.ModuleInstance.OnStartAsync();
                descriptor.State = GameModuleState.Started;
                GfyLog.Info($"模块就绪: {descriptor.Name} ({descriptor.ModuleType.Name})");
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

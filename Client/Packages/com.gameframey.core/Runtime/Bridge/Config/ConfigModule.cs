using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Asset;
using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Config
{
    /// <summary>配置模块：依赖 Asset 模块（其服务经作用域链可被构造注入）。</summary>
    [GameModule("Config", Priority = 900, Dependencies = new[] { "Asset" }, InstallerType = typeof(ConfigModuleInstaller))]
    public sealed class ConfigModule : GameModuleBase
    {
        private readonly IConfigService _service;

        public ConfigModule(IConfigService service)
        {
            _service = service;
        }

        public override UniTask OnInitializeAsync()
        {
            RegisterService(_service);
            return UniTask.CompletedTask;
        }
    }

    public sealed class ConfigModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IConfigService>(resolver =>
                new ConfigService(resolver.Resolve<IAssetService>()), Lifetime.Singleton);
        }
    }
}

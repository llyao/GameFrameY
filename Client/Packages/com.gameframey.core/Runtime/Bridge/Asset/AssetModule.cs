using Cysharp.Threading.Tasks;
using GameFrameY.Core.Module;
using VContainer;
using VContainer.Unity;

namespace GameFrameY.Bridge.Asset
{
    /// <summary>资源模块：把 IAssetService 注册进模块作用域（Bootstrap 阶段装配，服务惰性解析组件）。</summary>
    [GameModule("Asset", Priority = 1000, InstallerType = typeof(AssetModuleInstaller))]
    public sealed class AssetModule : GameModuleBase
    {
        private readonly IAssetService _service;

        public AssetModule(IAssetService service)
        {
            _service = service;
        }

        public override UniTask OnInitializeAsync()
        {
            RegisterService(_service);
            return UniTask.CompletedTask;
        }
    }

    public sealed class AssetModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IAssetService>(_ => new AssetService(), Lifetime.Singleton);
        }
    }
}

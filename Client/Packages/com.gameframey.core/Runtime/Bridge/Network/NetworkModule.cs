using Cysharp.Threading.Tasks;
using GameFrameY.Core.Module;
using VContainer;
using VContainer.Unity;

namespace GameFrameY.Bridge.Network
{
    [GameModule("Network", Priority = 700, InstallerType = typeof(NetworkModuleInstaller))]
    public sealed class NetworkModule : GameModuleBase
    {
        private readonly INetworkService _service;

        public NetworkModule(INetworkService service)
        {
            _service = service;
        }

        public override UniTask OnInitializeAsync()
        {
            RegisterService(_service);
            return UniTask.CompletedTask;
        }
    }

    public sealed class NetworkModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<INetworkService>(_ => new NetworkService(), Lifetime.Singleton);
        }
    }
}

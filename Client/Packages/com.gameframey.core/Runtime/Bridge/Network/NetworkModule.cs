using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Network
{
    [GameModule("Network", Priority = 700, InstallerType = typeof(NetworkModuleInstaller))]
    public sealed class NetworkModule : GameModuleBase
    {
    }

    public sealed class NetworkModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<INetworkService>(_ => new NetworkService(), Lifetime.Singleton);
        }
    }
}

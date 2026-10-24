using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Event
{
    [GameModule("Event", Priority = 1200, InstallerType = typeof(EventModuleInstaller))]
    public sealed class EventModule : GameModuleBase
    {
    }

    public sealed class EventModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IEventService>(_ => new EventService(), Lifetime.Singleton);
        }
    }
}

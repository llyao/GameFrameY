using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.UI
{
    [GameModule("UI", Priority = 800, InstallerType = typeof(UIModuleInstaller))]
    public sealed class UIModule : GameModuleBase
    {
    }

    public sealed class UIModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IUIService>(_ => new UIService(), Lifetime.Singleton);
        }
    }
}

using YooAsset;

namespace GameFrameY.Bridge
{
    /// <summary>
    /// 启动期配置（HostPlayMode 的资源服务器地址等）。PlayMode 本身在
    /// GameFrameYStartup.ConfigureGameFrameXOptions 里决定（发生在容器构建之前）。
    /// </summary>
    public sealed class GameFrameYLaunchSettings
    {
        public EPlayMode PlayMode { get; set; } = EPlayMode.EditorSimulateMode;

        /// <summary>资源服务器主地址（HostPlayMode 使用；EditorSimulateMode 忽略）。</summary>
        public string HostUrl { get; set; } = string.Empty;

        /// <summary>资源服务器备用地址。</summary>
        public string FallbackHostUrl { get; set; } = string.Empty;
    }
}

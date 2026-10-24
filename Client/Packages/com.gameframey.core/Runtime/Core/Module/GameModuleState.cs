namespace GameFrameY.Core.Module
{
    /// <summary>模块生命周期状态，按 Installed 列表顺序推进，卸载按逆序。</summary>
    public enum GameModuleState
    {
        Discovered = 0,
        Installing = 1,
        Initialized = 2,
        Started = 3,
        Shutdown = 4,
    }
}

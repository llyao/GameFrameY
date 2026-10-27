namespace GameFrameY.Core.Startup
{
    /// <summary>GameFrameY 启动阶段。状态只能沿定义的生命周期方向推进。</summary>
    public enum GameFrameYStartupState
    {
        Created = 0,
        Bootstrapping = 1,
        Patching = 2,
        LoadingConfig = 3,
        EnteringGame = 4,
        Ready = 5,
        Failed = 6,
        Stopping = 7,
        Stopped = 8,
    }
}

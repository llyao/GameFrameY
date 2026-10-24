namespace GameFrameY.Bridge
{
    /// <summary>GameFrameY 框架级事件 ID（走 GameFrameX EventComponent 的字符串事件）。</summary>
    public static class GameFrameYEventIds
    {
        /// <summary>启动流程全部完成（补丁 + 配置 + Deferred 模块装配）。</summary>
        public const string GameReady = "GameFrameY.GameReady";
    }
}

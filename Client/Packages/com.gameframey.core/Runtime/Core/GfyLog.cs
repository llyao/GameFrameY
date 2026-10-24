namespace GameFrameY.Core
{
    /// <summary>统一前缀日志（后续可替换为 GameFrameX/自研日志接口）。</summary>
    public static class GfyLog
    {
        private const string Prefix = "[GameFrameY] ";

        public static void Info(string message) => UnityEngine.Debug.Log(Prefix + message);

        public static void Warn(string message) => UnityEngine.Debug.LogWarning(Prefix + message);

        public static void Error(string message) => UnityEngine.Debug.LogError(Prefix + message);
    }
}

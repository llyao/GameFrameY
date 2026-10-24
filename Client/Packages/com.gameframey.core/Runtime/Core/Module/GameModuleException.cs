using System;

namespace GameFrameY.Core.Module
{
    /// <summary>模块体系（发现/排序/装配/卸载）相关异常。</summary>
    public class GameModuleException : Exception
    {
        public GameModuleException(string message) : base(message)
        {
        }

        public GameModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

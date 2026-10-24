namespace GameFrameY.Game.Modules.Demo
{
    /// <summary>示例业务服务接口：演示通过中央服务注册表向外提供服务。</summary>
    public interface IDemoGreetingService
    {
        string SayHello(string who);
    }

    public sealed class DemoGreetingService : IDemoGreetingService
    {
        public string SayHello(string who)
        {
            return $"Hello, {who}! 模块装配 → 服务发现 → 解析 全链路 OK";
        }
    }
}

using GameFrameX.Network.Runtime;
using GameFrameX.Runtime;

namespace GameFrameY.Bridge.Network
{
    /// <summary>
    /// 网络服务：包装 GameFrameX NetworkComponent（通道制）。
    /// 连接/发送在 INetworkChannel 上（Connect/Send/Call），消息协议由 Protobuf 生成代码提供。
    /// </summary>
    public interface INetworkService
    {
        INetworkChannel CreateChannel(string channelName, INetworkChannelHelper helper);

        INetworkChannel GetChannel(string channelName);

        bool HasChannel(string channelName);
    }

    public sealed class NetworkService : INetworkService
    {
        private static NetworkComponent Component => GameEntry.GetComponent<NetworkComponent>();

        public INetworkChannel CreateChannel(string channelName, INetworkChannelHelper helper)
        {
            return Component.CreateNetworkChannel(channelName, helper);
        }

        public INetworkChannel GetChannel(string channelName)
        {
            return Component.GetNetworkChannel(channelName);
        }

        public bool HasChannel(string channelName)
        {
            return Component.HasNetworkChannel(channelName);
        }
    }
}

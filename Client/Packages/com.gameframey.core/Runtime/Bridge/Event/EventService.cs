using System;
using GameFrameX.Event.Runtime;
using GameFrameX.Runtime;

namespace GameFrameY.Bridge.Event
{
    /// <summary>事件服务：包装 GameFrameX EventComponent 的字符串 ID 事件（订阅幂等）。</summary>
    public interface IEventService
    {
        void Subscribe(string eventId, EventHandler<GameEventArgs> handler);

        void Unsubscribe(string eventId, EventHandler<GameEventArgs> handler);

        /// <summary>派发空参数事件。</summary>
        void Fire(string eventId, object sender = null);

        void Fire(GameEventArgs args, object sender = null);
    }

    public sealed class EventService : IEventService
    {
        private static EventComponent Component => GameEntry.GetComponent<EventComponent>();

        public void Subscribe(string eventId, EventHandler<GameEventArgs> handler)
        {
            Component.CheckSubscribe(eventId, handler);
        }

        public void Unsubscribe(string eventId, EventHandler<GameEventArgs> handler)
        {
            Component.CheckUnsubscribe(eventId, handler);
        }

        public void Fire(string eventId, object sender = null)
        {
            Component.Fire(sender, eventId);
        }

        public void Fire(GameEventArgs args, object sender = null)
        {
            Component.Fire(sender, args);
        }
    }
}

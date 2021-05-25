using System;

namespace Plugins.Shared.UnityMonstackCore.Events
{
    public class EventSubscriberWithData<T> : IEventSubscriber
    {
        public object Target { get; }
        
        private readonly Action<T> m_callback; 

        public EventSubscriberWithData(Action<T> callback, object target = null)
        {
            Target = target;
            m_callback = callback;
        }

        public void Trigger(object data)
        {
            m_callback.Invoke((T)data);
        }
    }
}
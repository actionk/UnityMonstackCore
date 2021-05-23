using System;

namespace Plugins.Shared.UnityMonstackCore.Events
{
    public class EventSubscriber<T> : IEventSubscriber
    {
        public object Target { get; }
        
        private readonly Action m_callback; 

        public EventSubscriber(Action callback, object target = null)
        {
            Target = target;
            m_callback = callback;
        }

        public void Trigger(object data)
        {
            m_callback.Invoke();
        }
    }
}
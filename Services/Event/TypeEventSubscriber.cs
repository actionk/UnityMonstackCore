using System;

namespace Plugins.Shared.UnityMonstackCore.Services.Event
{
    public class TypeEventSubscriber<T> : ITypedEventSubscriber
    {
        public object Target { get; }
        
        private readonly Action m_callback; 

        public TypeEventSubscriber(Action callback, object target = null)
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
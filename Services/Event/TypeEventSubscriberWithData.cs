using System;

namespace Plugins.Shared.UnityMonstackCore.Services.Event
{
    public class TypeEventSubscriberWithData<T> : ITypedEventSubscriber
    {
        public object Target { get; }
        
        private readonly Action<T> m_callback; 

        public TypeEventSubscriberWithData(Action<T> callback, object target = null)
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
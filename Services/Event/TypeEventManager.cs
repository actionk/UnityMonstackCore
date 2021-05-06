using System;
using System.Reflection;
using PixelCrushers;
using Plugins.Shared.UnityMonstackCore.Utils;

namespace Plugins.Shared.UnityMonstackCore.Services.Event
{
    public class TypeEventManager
    {
        private readonly MultiValueDictionary<Type, ITypedEventSubscriber> m_eventSubscribers = new MultiValueDictionary<Type, ITypedEventSubscriber>();

        public ITypedEventSubscriber CreateListener<T>(Action<T> callback, object target = null)
        {
            var eventSubscriber = new TypeEventSubscriberWithData<T>(callback, target);
            m_eventSubscribers.Add(typeof(T), eventSubscriber);
            return eventSubscriber;
        }

        public ITypedEventSubscriber CreateListener<T>(Action callback, object target = null)
        {
            var eventSubscriber = new TypeEventSubscriber<T>(callback, target);
            m_eventSubscribers.Add(typeof(T), eventSubscriber);
            return eventSubscriber;
        }

        public void RemoveSubscribersForEvent<T>()
        {
            m_eventSubscribers.Remove(typeof(T));
        }

        public void RemoveSubscribersForTarget(object target)
        {
            foreach (var hashSet in m_eventSubscribers)
                hashSet.Value.RemoveWhere(x => x.Target == target);
        }

        public void RemoveSubscribersForTargetAssembly(Assembly assembly)
        {
            m_eventSubscribers.RemoveAll(x => x.Assembly == assembly);
        }

        public void DispatchEvent<T>()
        {
            DispatchEvent<T>(default);
        }

        public void DispatchEvent<T>(T data)
        {
            var eventSubscribers = m_eventSubscribers.GetValues(typeof(T), false);
            if (eventSubscribers == null)
                return;

            foreach (var subscriber in eventSubscribers)
                subscriber.Trigger(data);
        }
    }
}
using System;
using System.Reflection;
using Plugins.UnityMonstackCore.Services.Event;

namespace Plugins.Shared.UnityMonstackCore.Services.Event
{
    public class AEventManager<T>
    {
        private readonly EventDispatcher<T> m_eventDispatcher = new EventDispatcher<T>();

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, object target = null)
            where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(m_eventDispatcher, eventId, target);
        }

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, object target,
            Action<TValue> action) where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(m_eventDispatcher, eventId, target).Subscribe(action);
        }

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, Action<TValue> action)
            where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(m_eventDispatcher, eventId).Subscribe(action);
        }

        public EventSubscriber<T> CreateListener(T eventId, object target = null)
        {
            return new EventSubscriber<T>(m_eventDispatcher, eventId, target);
        }

        public EventSubscriber<T> CreateListener(T eventId, object target, Action action)
        {
            return new EventSubscriber<T>(m_eventDispatcher, eventId, target).Subscribe(action);
        }

        public EventSubscriber<T> CreateListener(T eventId, Action action)
        {
            return new EventSubscriber<T>(m_eventDispatcher, eventId).Subscribe(action);
        }

        public void RemoveSubscribersForEvent(T eventId)
        {
            m_eventDispatcher.RemoveSubscribersForEvent(eventId);
        }

        public void RemoveSubscribersForTarget(object target)
        {
            m_eventDispatcher.RemoveSubscribersForTarget(target);
        }

        public void RemoveSubscribersForTargetAssembly(Assembly assembly)
        {
            m_eventDispatcher.RemoveSubscribersForTargetAssembly(assembly);
        }

        public void DispatchEvent<TValue>(T eventId, TValue value) where TValue : class
        {
            m_eventDispatcher.DispatchEvent(eventId, value);
        }

        public void DispatchEvent(T eventId)
        {
            m_eventDispatcher.DispatchEvent(eventId);
        }

        public void PrintSubscribers()
        {
            m_eventDispatcher.PrintSubscribers();
        }
    }
}
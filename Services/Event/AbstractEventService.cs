using System;
using System.Reflection;

namespace Plugins.UnityMonstackCore.Services.Event
{
    public class AbstractEventService<T>
    {
        private readonly EventDispatcher<T> eventDispatcher = new EventDispatcher<T>();

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, object target = null)
            where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(eventDispatcher, eventId, target);
        }

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, object target,
            Action<TValue> action) where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(eventDispatcher, eventId, target).Subscribe(action);
        }

        public EventSubscriberWithData<T, TValue> CreateListener<TValue>(T eventId, Action<TValue> action)
            where TValue : class
        {
            return new EventSubscriberWithData<T, TValue>(eventDispatcher, eventId).Subscribe(action);
        }

        public EventSubscriber<T> CreateListener(T eventId, object target = null)
        {
            return new EventSubscriber<T>(eventDispatcher, eventId, target);
        }

        public EventSubscriber<T> CreateListener(T eventId, object target, Action action)
        {
            return new EventSubscriber<T>(eventDispatcher, eventId, target).Subscribe(action);
        }

        public EventSubscriber<T> CreateListener(T eventId, Action action)
        {
            return new EventSubscriber<T>(eventDispatcher, eventId).Subscribe(action);
        }

        public void RemoveSubscribersForEvent(T eventId)
        {
            eventDispatcher.RemoveSubscribersForEvent(eventId);
        }

        public void RemoveSubscribersForTarget(object target)
        {
            eventDispatcher.RemoveSubscribersForTarget(target);
        }

        public void RemoveSubscribersForTargetAssembly(Assembly assembly)
        {
            eventDispatcher.RemoveSubscribersForTargetAssembly(assembly);
        }

        public void DispatchEvent<TValue>(T eventId, TValue value) where TValue : class
        {
            eventDispatcher.DispatchEvent(eventId, value);
        }

        public void DispatchEvent(T eventId)
        {
            eventDispatcher.DispatchEvent(eventId);
        }

        public void PrintSubscribers()
        {
            eventDispatcher.PrintSubscribers();
        }
    }
}
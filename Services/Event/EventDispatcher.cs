using System.Collections.Generic;
using System.Reflection;
using Plugins.UnityMonstackCore.Extensions.Collections;
using Plugins.UnityMonstackCore.Loggers;

namespace Plugins.UnityMonstackCore.Services.Event
{
    public class EventDispatcher<T>
    {
        private readonly Dictionary<T, HashSet<EventSubscriber<T>>> m_subscribers =
            new Dictionary<T, HashSet<EventSubscriber<T>>>();

        public void AddListener(EventSubscriber<T> subscriber)
        {
            var setOfSubscribers =
                m_subscribers.ComputeIfAbsent(subscriber.EventKey, eventKey => new HashSet<EventSubscriber<T>>());
            setOfSubscribers.Add(subscriber);
        }

        public void RemoveListener(EventSubscriber<T> subscriber)
        {
            if (m_subscribers.ContainsKey(subscriber.EventKey)) m_subscribers[subscriber.EventKey].Remove(subscriber);
        }

        public void DispatchEvent<TValue>(T eventId, TValue value) where TValue : class
        {
            if (!m_subscribers.ContainsKey(eventId)) return;

            var subscriberSet = new HashSet<EventSubscriber<T>>(m_subscribers[eventId]);
            foreach (var subscriber in subscriberSet)
            {
                subscriber.Dispatch();

                var eventSubscriberWithData = subscriber as EventSubscriberWithData<T, TValue>;
                eventSubscriberWithData?.Dispatch(value);
            }
        }

        public void DispatchEvent(T eventId)
        {
            if (!m_subscribers.ContainsKey(eventId)) return;

            var subscribersForEvent = new HashSet<EventSubscriber<T>>(m_subscribers[eventId]);
            foreach (var subscriber in subscribersForEvent)
                subscriber.Dispatch();
        }

        public void RemoveSubscribersForEvent(T eventId)
        {
            m_subscribers.Remove(eventId);
        }

        public void RemoveSubscribersForTarget(object target)
        {
            m_subscribers.ForEachValue(setOfSubscribers =>
                setOfSubscribers.RemoveWhere(subscriber => subscriber.Target == target)
            );
        }

        public void RemoveSubscribersForTargetAssembly(Assembly assembly)
        {
            m_subscribers.ForEachValue(setOfSubscribers =>
                setOfSubscribers.RemoveWhere(subscriber => subscriber.Target != null && subscriber.Target.GetType().Assembly == assembly)
            );
        }

        public void PrintSubscribers()
        {
#if UNITY_EDITOR
            foreach (var entry in m_subscribers)
            {
                if (entry.Value.Count == 0)
                    continue;
                
                UnityLogger.Log($"Type: {entry.Key}, subscribers: {entry.Value.Count}");
            }
#endif
        }
    }
}
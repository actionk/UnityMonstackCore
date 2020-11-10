using System.Collections.Generic;
using Plugins.UnityMonstackCore.Extensions.Collections;

namespace Plugins.UnityMonstackCore.Services.Event
{
    public class EventDispatcher<T>
    {
        private readonly Dictionary<T, HashSet<EventSubscriber<T>>> subscribers =
            new Dictionary<T, HashSet<EventSubscriber<T>>>();

        public void AddListener(EventSubscriber<T> subscriber)
        {
            var setOfSubscribers =
                subscribers.ComputeIfAbsent(subscriber.EventKey, eventKey => new HashSet<EventSubscriber<T>>());
            setOfSubscribers.Add(subscriber);
        }

        public void RemoveListener(EventSubscriber<T> subscriber)
        {
            if (subscribers.ContainsKey(subscriber.EventKey)) subscribers[subscriber.EventKey].Remove(subscriber);
        }

        public void DispatchEvent<TValue>(T eventId, TValue value) where TValue : class
        {
            if (!subscribers.ContainsKey(eventId)) return;

            var subscriberSet = new HashSet<EventSubscriber<T>>(subscribers[eventId]);
            foreach (var subscriber in subscriberSet)
            {
                subscriber.Dispatch();

                var eventSubscriberWithData = subscriber as EventSubscriberWithData<T, TValue>;
                eventSubscriberWithData?.Dispatch(value);
            }
        }

        public void DispatchEvent(T eventId)
        {
            if (!subscribers.ContainsKey(eventId)) return;

            var subscribersForEvent = new HashSet<EventSubscriber<T>>(subscribers[eventId]);
            foreach (var subscriber in subscribersForEvent)
                subscriber.Dispatch();
        }

        public void RemoveSubscribersForEvent(T eventId)
        {
            subscribers.Remove(eventId);
        }

        public void RemoveSubscribersForTarget(object target)
        {
            subscribers.ForEachValue(setOfSubscribers =>
                setOfSubscribers.RemoveWhere(subscriber => subscriber.Target == target)
            );
        }
    }
}
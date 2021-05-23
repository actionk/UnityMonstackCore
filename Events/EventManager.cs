using System;
using System.Reflection;
using PixelCrushers;
using Plugins.Shared.UnityMonstackCore.Utils;
using Plugins.UnityMonstackCore.Loggers;

namespace Plugins.Shared.UnityMonstackCore.Events
{
    public class EventManager
    {
        private readonly MultiValueDictionary<Type, IEventSubscriber> m_eventSubscribers = new MultiValueDictionary<Type, IEventSubscriber>();

#region Builder

        /// <summary>
        /// Use <c>Builder</c> when you want to make multiple subscribes in one expression:
        /// <code>
        ///  DependencyProvider.Resolve<ClientEventManager>().Builder
        ///    .CreateListener<ItemSlotDragDropStartedEvent>(OnDragDropStart, this)
        ///    .CreateListener<ItemSlotDragDropFinishedEvent>(OnDragDropStop, this)
        ///    .CreateListener<MouseOverEnterEvent>(OnMouseOverEnter, this)
        ///    .CreateListener<MouseOverLeaveEvent>(OnMouseOverLeave, this);
        /// </code>
        /// </summary>
        public class InnerBuilder
        {
            private readonly EventManager m_eventManager;

            public InnerBuilder(EventManager eventManager)
            {
                m_eventManager = eventManager;
            }

            public InnerBuilder CreateListener<T>(Action<T> callback, object target = null)
            {
                m_eventManager.CreateListener<T>(callback, target);
                return this;
            }

            public InnerBuilder CreateListener<T>(Action callback, object target = null)
            {
                m_eventManager.CreateListener<T>(callback, target);
                return this;
            }
        }

        public InnerBuilder Builder { get; }

#endregion

        public EventManager()
        {
            Builder = new InnerBuilder(this);
        }

        public IEventSubscriber CreateListener<T>(Action<T> callback, object target = null)
        {
            var eventSubscriber = new EventSubscriberWithData<T>(callback, target);
            m_eventSubscribers.Add(typeof(T), eventSubscriber);
            return eventSubscriber;
        }

        public IEventSubscriber CreateListener<T>(Action callback, object target = null)
        {
            var eventSubscriber = new EventSubscriber<T>(callback, target);
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

        public void PrintSubscribers()
        {
#if UNITY_EDITOR
            foreach (var entry in m_eventSubscribers)
            {
                if (entry.Value.Count == 0)
                    continue;

                UnityLogger.Log($"Type: {entry.Key}, subscribers: {entry.Value.Count}");
            }
#endif
        }
    }
}
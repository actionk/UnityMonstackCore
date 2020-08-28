using System;

namespace Plugins.UnityMonstackCore.Services.Event
{
    public class EventSubscriber<TEventKey>
    {
        protected readonly EventDispatcher<TEventKey> dispatcher;
        private Action action;

        public EventSubscriber(EventDispatcher<TEventKey> dispatcher, TEventKey eventKey, object target = null)
        {
            EventKey = eventKey;
            this.dispatcher = dispatcher;
            Target = target;
        }

        public TEventKey EventKey { get; }

        public object Target { get; }

        public EventSubscriber<TEventKey> Subscribe(Action action)
        {
            this.action = action;
            dispatcher.AddListener(this);
            return this;
        }

        public EventSubscriber<TEventKey> Unsubscribe()
        {
            dispatcher.RemoveListener(this);
            return this;
        }

        public void Dispatch()
        {
            if (action != null) action.Invoke();
        }
    }
}
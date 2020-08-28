using System;

namespace Plugins.UnityMonstackCore.Services.Event
{
    public class EventSubscriberWithData<TEventKey, TValue> : EventSubscriber<TEventKey> where TValue : class
    {
        private Action<TValue> action;

        public EventSubscriberWithData(EventDispatcher<TEventKey> dispatcher, TEventKey eventKey, object target = null)
            : base(dispatcher, eventKey, target)
        {
        }

        public EventSubscriberWithData<TEventKey, TValue> Subscribe(Action<TValue> action)
        {
            this.action = action;
            dispatcher.AddListener(this);
            return this;
        }

        public void Dispatch(TValue value)
        {
            if (action != null) action.Invoke(value);
        }
    }
}
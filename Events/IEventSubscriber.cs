namespace Plugins.Shared.UnityMonstackCore.Events
{
    public interface IEventSubscriber
    {
        object Target { get; }
        void Trigger(object data);
    }
}
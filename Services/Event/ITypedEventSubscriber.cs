namespace Plugins.Shared.UnityMonstackCore.Services.Event
{
    public interface ITypedEventSubscriber
    {
        object Target { get; }
        void Trigger(object data);
    }
}
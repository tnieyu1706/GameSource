using _Project.Scripts.Interact;

namespace _Project.Scripts.Item
{
    public interface IUnsubscribeInteractable
    {
        void Unsubscribe(IInteractor interactor);
    }

    public interface IUnsubscribeInteractable<in T> : IUnsubscribeInteractable
        where T : IInteractor
    {
        void IUnsubscribeInteractable.Unsubscribe(IInteractor interactor)
        {
            Unsubscribe((T)interactor);
        }
        void Unsubscribe(T interactor);
    }
        
}
namespace _Project.Scripts.Interact
{
    public interface IInteractor
    {
        void Visit(IInteractable interactable);
    }

    public interface IInteractor<in TInteractable> : IInteractor
        where TInteractable : IInteractable
    {
        void IInteractor.Visit(IInteractable interactable)
        {
            Visit((TInteractable)interactable);
        }
        void Visit(TInteractable interactable);
    }
}



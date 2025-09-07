namespace _Project.Scripts.Interact
{
    public interface IInteractable
    {
        void Accept(IInteractor interactor);
    }

    public interface IInteractable<in TInteractor> : IInteractable
        where TInteractor : IInteractor
    {
        void IInteractable.Accept(IInteractor interactor)
        {
            Accept((TInteractor)interactor);
        }
        void Accept(TInteractor interactor);
    }
}
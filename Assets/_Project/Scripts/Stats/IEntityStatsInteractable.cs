using _Project.Scripts.Interact;

namespace _Project.Scripts.Stats
{
    public interface IEntityStatsInteractable : IInteractable<IEntityStatsInteractor>
    {
        public IEntityStatsData EntityStats { get; }
    }
}
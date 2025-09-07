using _Project.Scripts.Interact;

namespace _Project.Scripts.Stats
{
    public interface IEntityStatsInteractor : IInteractor
    {
        public void HandleEntityStats(IEntityStatsData entityStats);
    }
}
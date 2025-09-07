using _Project.Scripts.Stats;

namespace _Project.Scripts.Item
{
    public interface IItemStatsUser : IItemUser
    {
        IEntityStatsable EntityStatsable { get; }
    }
}
namespace _Project.Scripts.Item
{
    /// <summary>
    /// you have to override Awake to get id from IdentifySystemHelper
    /// </summary>
    public interface IBaseTypeData
    {
        int TypeId { get; set; }
        string Name { get; }
    }
}
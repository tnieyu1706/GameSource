using System;
using _Project.Scripts.Utils;

namespace _Project.Scripts.Persistence
{
    /// <summary>
    /// 1. Implement for data
    /// 2.When implement Id myst be specify attribute [field: Serialize]
    /// to storage to save.
    /// </summary>
    public interface IPersistenceSavable
    {
        SerializableGuid Id { get; set; }
    }
}
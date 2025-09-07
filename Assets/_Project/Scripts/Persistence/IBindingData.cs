using System;
using _Project.Scripts.Utils;
using Unity.VisualScripting;

namespace _Project.Scripts.Persistence
{
    public interface IBindingData
    {
        void Bind(IPersistenceSavable data);
        void SaveEntity();
        void LoadEntity();
    }
    /// <summary>
    /// 1. Implement for Entity
    /// 2. When create Id must be create
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IBindingData<TData> : IBindingData
        where TData : IPersistenceSavable
    {
        SerializableGuid Id { get; set; }
        TData Data { get; set; }

        void IBindingData.Bind(IPersistenceSavable data)
        {
            Bind(data);
        }

        void Bind(TData data);
    }

    public static class InterfaceBindingDataExtensions
    {
        public static void BindDefault<TData>(this IBindingData<TData> bindingData, TData data)
            where TData : IPersistenceSavable
        {
            bindingData.Data = data;
            data.Id = bindingData.Id;
        }
    }
}
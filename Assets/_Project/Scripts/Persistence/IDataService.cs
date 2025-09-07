using System;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Persistence
{
    public interface IDataService<T> : IInterfaceShowInspector
    {
        void Save(string name, T data, bool overwrite = true);
        T Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}
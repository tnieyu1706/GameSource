using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Item
{
    public static class MyAddressableSystemHelper
    {
        public static AsyncOperationHandle<T> LoadAsyncIdentify<T>(int id)
            where T : IBaseTypeData
        {
            string addressableName = $"{typeof(T).Name}_{id}";

            return Addressables.LoadAssetAsync<T>(addressableName);
        }

        public static AsyncOperationHandle<IList<T>> LoadAsyncIdentifies<T>(
            Addressables.MergeMode mergeMode,
            Action<T> callback = null,
            params string[] labels
        )
            where T : IBaseTypeData
        {
            return Addressables.LoadAssetsAsync<T>(labels, callback, mergeMode);
        }
    }
}
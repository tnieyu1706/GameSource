using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using _Project.Scripts.Item;
using UnityEngine;
#endif

namespace _Project.Scripts.Config
{
    internal class MyAssetModificationProcessor : AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            UnityEngine.Debug.Log("OnWillDeleteAsset: " + assetPath);

#if UNITY_EDITOR
            var objData = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (objData != null && objData is IBaseTypeData baseTypeData)
            {
                Debug.Log($"working with {baseTypeData.GetType().Name}");
                Type actualType = baseTypeData.GetType();
                
                IdentifySystemHelper.RemoveIdentify(actualType, baseTypeData.TypeId);
                
                Debug.Log($"removing {baseTypeData.GetType().Name}");
            }
            
#endif
            return AssetDeleteResult.DidNotDelete;
            // Có 3 option:
            // DidNotDelete   = Unity tiếp tục xoá
            // FailedDelete   = Unity cancel xoá
            // DidDeleteCustom = Bạn tự xử lý xoá
        }

        // Gọi khi asset được tạo mới
        static void OnWillCreateAsset(string assetPath)
        {
            UnityEngine.Debug.Log("OnWillCreateAsset: " + assetPath);
        }

        // Gọi khi asset được di chuyển/rename
        static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            UnityEngine.Debug.Log($"OnWillMoveAsset: {oldPath} -> {newPath}");
            
            var objData = AssetDatabase.LoadAssetAtPath<Object>(newPath);
            if (objData is IBaseTypeData baseTypeData)
            {
                var type = baseTypeData.GetType();

                // Xoá entry cũ nếu còn tồn tại
                IdentifySystemHelper.RemoveIdentify(type, baseTypeData.TypeId);

                // Re-add vào registry theo TypeId cũ
                if (!IdentifySystemHelper.Registries.ContainsKey(type))
                    IdentifySystemHelper.Registries[type] = new IdentifyManager();

                IdentifySystemHelper.Registries[type].Identifiers[baseTypeData.TypeId] = objData;
            }
            
            return AssetMoveResult.DidNotMove;
            // Có thể return FailedMove để cancel
        }
    }
}
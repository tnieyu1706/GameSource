using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.CraftingSystem;
using _Project.Scripts.Item;
using _Project.Scripts.Utils.MyAttribute;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TestAddressable : MonoBehaviour
{
    [SerializeField] private Image imageTemp;

    [SerializeField] List<RecipeSo> recipes = new List<RecipeSo>();

    void Start()
    {
        var operationHandle = MyAddressableSystemHelper.LoadAsyncIdentifies<RecipeSo>(
            Addressables.MergeMode.Intersection,
            r => Debug.Log($"Completed Load : {r.Name}"),
            "Recipe"
        );

        operationHandle.Completed += (o) =>
        {
            if (o.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Completed Load All data.");
                recipes = o.Result.ToList();
            }
        };
    }
}
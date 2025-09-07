using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Item;
using AwesomeAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace _Project.Scripts.CraftingSystem
{
    public class RecipeManager : BehaviorSingleton<RecipeManager>
    {
        public List<RecipeSo> recipes = new List<RecipeSo>();
        [Required] public ItemTypeData trashItem;

        private void OnEnable()
        {
            MyAddressableSystemHelper.LoadAsyncIdentifies<RecipeSo>(
                Addressables.MergeMode.Intersection,
                r => recipes.Add(r),
                GlobalConfigs.RecipeGroupAddressable
            ).Completed += oh => Debug.Log("Completed load Recipes");
        }

        private void OnDisable()
        {
            recipes?.Clear();
        }

        public List<RecipeSo> GetRecipesWithProgressingType(ProgressingType progressingType)
        {
            return recipes?.Where(r => r.progressingType == progressingType).ToList();
        }

        public List<RecipeSo> GetRecipesWithMachineType(MachineType machineType)
        {
            return recipes?.Where(r => r.machineType == machineType).ToList();
        }

        public List<RecipeSo> GetRecipesWithMachineAndProgressingType(MachineType machineType,
            ProgressingType progressingType)
        {
            return recipes?.Where(r =>
                r.progressingType == progressingType && r.machineType == machineType
            ).ToList();
        }

        public RecipeSo GetRecipeWithIngredientsBySpecifyRecipes(List<RecipeSo> recipeSos, List<ItemTypeData> items)
        {
            return recipeSos.FirstOrDefault(r =>
            {
                Debug.Log($"inputComponents count : {r.inputComponents.Count}, items count : {items.Count}");
                bool found = r.inputComponents.Count == items.Count;
                if (!found) return false;

                items.Sort(SingletonFactory.GetInstance<ItemTypeComparer>());

                var sortedInputs = r.inputComponents.ToList();
                sortedInputs.Sort(SingletonFactory.GetInstance<ItemTypeComparer>());

                for (int i = 0; i < sortedInputs.Count; i++)
                {
                    Debug.Log($"sorted: {sortedInputs[i].Name} - item: {items[i].Name}");
                    if (items[i].TypeId != sortedInputs[i].TypeId)
                    {
                        return false;
                    }
                }

                return true;
            });
        }
    }
}
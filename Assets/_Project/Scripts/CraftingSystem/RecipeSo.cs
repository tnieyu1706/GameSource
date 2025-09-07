using System;
using System.Collections.Generic;
using _Project.Scripts.Config;
using _Project.Scripts.Item;
using AwesomeAttributes;
using UnityEngine;

namespace _Project.Scripts.CraftingSystem
{
    public enum ProgressingType
    {
        Handler,
        Cooking,
        Alchemy,
        Brew,
        Forging,
        Magic,
        Crafting
    }

    public enum MachineType
    {
        Handler_CuttingBoard,
        Handler_Mixer,
        
        Cooking_Stove,
        Cooking_Oven,
        Cooking_Steamer,
        
        Alchemy_FermentationBarrel,
        Alchemy_AgingBarrel,
        Alchemy_Extractor,
        Alchemy_DistillationTable,
        
        Brew_JuiceMaker,
        Brew_Grinder,
        
        Forging_Anvil,
        Forging_Kiln,
        
        Magic_RunCarvingTable,
        Magic_Cauldron,
        Magic_MagicStove,
        
        //crafting => special.
        Crafting_CraftingTable,
        Crafting_ManualWorkbench,
        Crafting_AlchemicalWorkbench,
        Crafting_ProcessingBench,
        Crafting_MagicTable
    }
    
    [CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/Crafting System/RecipeSO")]
    public class RecipeSo : ScriptableObject, IBaseTypeData
    {
        [SerializeField, Readonly] private int recipeID;
        [SerializeField] private string name;
        
        public List<ItemTypeData> inputComponents;
        public ProgressingType progressingType;
        public MachineType machineType;
        public ItemTypeData outputComponent;

        public float executeSecondTime;

        public int TypeId
        {
            get => recipeID;
            set => recipeID = value;
        }

        public string Name => name;

        private void OnValidate()
        {
            int maxInputs = GlobalConfigs.RecipeMaxComponentInputs;
            if (inputComponents.Count > maxInputs)
            {
                Debug.Log($"Recipe only have max inputs = {maxInputs}");
                inputComponents.Resize(maxInputs);
            }
        }

        void Awake()
        {
            TypeId = IdentifySystemHelper.GenerateId(this);
        }
    }
}
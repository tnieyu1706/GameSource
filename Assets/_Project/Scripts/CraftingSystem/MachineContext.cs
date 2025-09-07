using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.Patterns.Builder;
using _Project.Scripts.General.StaticHelpers;
using _Project.Scripts.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.CraftingSystem
{
    public interface IMachineContext
    {
        ProgressingType ProgressingType { get; }
        MachineType MachineType { get; }
        MachineState CurrentState { get; set; }
        MachineStateHolder StateHolder { get; }
        IItemUser CurrentItemUser { get; }
        int SlotComponent { get; }
    }

    [Serializable]
    public class MachineStateHolder
    {
        public IdleMachineState IdleState;
        public WorkingMachineState WorkingState;
        public CompletedMachineState CompletedState;

        private MachineStateHolder()
        {
        }

        public class MachineStateHolderBuilder : IMyBuilder<MachineStateHolder>
        {
            private MachineStateHolder _machineStateHolder;

            public MachineStateHolderBuilder()
            {
                _machineStateHolder = new MachineStateHolder();
            }

            public MachineStateHolderBuilder BuildIdleState(
                MachineContext context,
                Action onInitUI,
                Action<Stack<ItemTypeData>> onUpdateUI,
                Action onExitUI
            )
            {
                _machineStateHolder.IdleState =
                    new IdleMachineState(
                        context,
                        onInitUI,
                        onUpdateUI,
                        onExitUI
                    );
                return this;
            }

            public MachineStateHolderBuilder BuildWorkingState(MachineContext context, Image progressingBar,
                float progressScale = 1f)
            {
                _machineStateHolder.WorkingState = new WorkingMachineState(context, progressingBar, progressScale);
                return this;
            }

            public MachineStateHolderBuilder BuildCompletedState(MachineContext context, Image itemCompletedIcon)
            {
                _machineStateHolder.CompletedState = new CompletedMachineState(context, itemCompletedIcon);
                return this;
            }

            public MachineStateHolder Build()
            {
                return _machineStateHolder;
            }
        }
    }

    public class MachineContext : MonoBehaviour, IMachineContext, IItemInteractable, IUnsubscribeInteractable<IItemUser>
    {
        #region ConfigProperties

        [FoldoutGroup("Configurations")] [SerializeField] [Range(1, GlobalConfigs.RecipeMaxComponentInputs)]
        private int slotComponent = 2;

        [FoldoutGroup("Configurations")] public float progressScale = 1f;

        [FoldoutGroup("Configurations")] [SerializeField]
        private ProgressingType progressingType;

        [FoldoutGroup("Configurations")] [SerializeField]
        private MachineType machineType;

        #endregion

        #region ContextProperties

        [FoldoutGroup("Context Variables")] [SerializeReference]
        private MachineState currentState;

        [FoldoutGroup("Context Variables")] [SerializeField]
        private MachineStateHolder stateHolder;

        #endregion

        #region GUIProperties

        [FoldoutGroup("GUI Variables")] [SerializeField, AwesomeAttributes.Required]
        private RectTransform ingredientsContainer;

        private List<GameObject> _ingredients;

        [FoldoutGroup("GUI Variables")] [SerializeField, AwesomeAttributes.Required]
        private GameObject ingredientPrefab;

        [FoldoutGroup("GUI Variables")] [SerializeField]
        private float ingredientSpacing = 0.5f;

        [FoldoutGroup("GUI Variables")] [SerializeField, AwesomeAttributes.Required]
        private Image progressingBar;

        [FoldoutGroup("GUI Variables")] [SerializeField, AwesomeAttributes.Required]
        private Image itemCompletedIcon;

        #endregion

        public MachineState CurrentState
        {
            get => currentState;
            set => currentState = value;
        }

        public MachineStateHolder StateHolder => stateHolder;

        public IItemUser CurrentItemUser { get; private set; }

        public int SlotComponent => slotComponent;

        public ProgressingType ProgressingType => progressingType;
        public MachineType MachineType => machineType;

        public float ProgressScale
        {
            get => progressScale;
            set
            {
                progressScale = value;
                if (stateHolder != null && stateHolder.WorkingState != null)
                {
                    stateHolder.WorkingState.progressScale = progressScale;
                }
            }
        }

        void Awake()
        {
            if (ingredientsContainer == null || ingredientPrefab == null)
            {
                Debug.LogError("Ingredient Container or Ingredient Prefab is null");
                return;
            }

            GenerateIngredientsUI(SlotComponent);

            stateHolder = new MachineStateHolder.MachineStateHolderBuilder()
                .BuildIdleState(
                    this,
                    OnIdleStateUIInit,
                    OnIdleStateUIUpdate,
                    OnIdleStateUIExit
                )
                .BuildWorkingState(this, progressingBar, ProgressScale)
                .BuildCompletedState(this, itemCompletedIcon)
                .Build();
        }

        void Start()
        {
            currentState = stateHolder.IdleState;
            currentState.Entry();
        }

        #region IdleStateConfig

        void OnIdleStateUIInit()
        {
            ingredientsContainer.gameObject.SetActive(true);

            foreach (var ingredient in _ingredients)
            {
                ingredient.SetActive(false);
            }
        }

        void OnIdleStateUIUpdate(Stack<ItemTypeData> stack)
        {
            List<ItemTypeData> itemStack = stack.ToList();
            itemStack.Reverse();
            int enableIngredientsCount = stack.Count;
            List<GameObject> enableIngredients = new List<GameObject>(enableIngredientsCount);
            List<GameObject> disableIngredients = new List<GameObject>(_ingredients.Count - enableIngredientsCount);

            for (int i = 0; i < _ingredients.Count; i++)
            {
                if (i < enableIngredientsCount)
                {
                    enableIngredients.Add(_ingredients[i]);
                }
                else
                {
                    disableIngredients.Add(_ingredients[i]);
                }
            }

            var (firstPosition, perSpacing, ingredientPrefabRect) =
                CalculateIngredientProperties(enableIngredients.Count);

            int enableIndex = 0;
            UIMethodHelper.HandleEnableDisableElements(
                enableIngredients,
                (e) =>
                {
                    if (!e.activeSelf)
                    {
                        e.SetActive(true);
                        e.GetComponent<Image>().sprite = itemStack[enableIndex].icon;
                    }

                    e.GetComponent<RectTransform>().anchoredPosition =
                        ingredientPrefabRect.anchoredPosition
                            .With(x: firstPosition + perSpacing * enableIndex);

                    enableIndex++;
                },
                disableIngredients,
                (d) => { d.gameObject.SetActive(false); }
            );
        }

        void OnIdleStateUIExit()
        {
            ingredientsContainer.gameObject.SetActive(false);
        }

        #endregion

        #region UIGenerate

        void GenerateIngredientsUI(int ingredientNumber)
        {
            _ingredients = new List<GameObject>();
            for (int i = 0; i < ingredientNumber; i++)
            {
                GameObject ingredient = Instantiate(ingredientPrefab, ingredientsContainer);
                _ingredients.Add(ingredient);
            }

            var (firstPosition, perInputPositionSpacing, ingredientPrefabRect) =
                CalculateIngredientProperties(_ingredients.Count);
            int index = 0;

            UIMethodHelper.HandleEnableDisableElements(
                _ingredients,
                (obj) =>
                {
                    obj.GetComponent<RectTransform>().anchoredPosition =
                        ingredientPrefabRect.anchoredPosition
                            .With(x: firstPosition + perInputPositionSpacing * index);
                    obj.name = $"Ingredient_{index}";

                    index++;
                }
            );

            ingredientPrefab.SetActive(false);
        }

        (float, float, RectTransform) CalculateIngredientProperties(int ingredientNumber)
        {
            return UIMethodHelper.CalculateHorizontalGridPosition(ingredientNumber, ingredientSpacing,
                ingredientPrefab);
        }

        #endregion

        void Update()
        {
            CurrentState?.Update();
        }

        public void Accept(IItemUser interactor)
        {
            CurrentItemUser = interactor;
            CurrentState?.SubscribeInteraction();
        }

        public void Unsubscribe(IItemUser interactor)
        {
            CurrentState?.UnsubscribeInteraction();
            CurrentItemUser = null;
        }
    }
}
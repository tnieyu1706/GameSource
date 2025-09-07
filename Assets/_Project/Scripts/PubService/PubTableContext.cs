using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.General.Patterns.Builder;
using _Project.Scripts.General.StaticHelpers;
using _Project.Scripts.Item;
using _Project.Scripts.Storage;
using _Project.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.PubService
{
    public interface IPubTableContext : IBaseTypeData
    {
        PubTableState CurrentState { get; set; }

        //temp
        string ClientName { get; set; }

        MoneyTaskRating Rating { get; set; }

        //link itemInStorageWithUI.
        IStorage OrderItems { get; }

        //config when orderItems Set by manager.
        float TotalOrderItemsPrice { get; set; }
        IItemUser CurrentItemUser { get; set; }
        PubTableStateHolder PubTableStateHolder { get; }

        void ChangeState(PubTableState state);
    }

    [Serializable]
    public class MoneyTaskRating
    {
        public float orderTaskRating = 0f;
        public float waitingTaskRating = 0f;
        private static readonly float TotalTaskDivide = 2f;

        public float GetMoneyRating()
        {
            return (orderTaskRating + waitingTaskRating) / TotalTaskDivide;
        }

        public static float ConvertProgressToRating(float progress)
        {
            return progress switch
            {
                <= 0 => 0,
                < 0.3f => 0.4f,
                < 0.5f => 0.7f,
                < 0.8f => 1f,
                >= 0.8f => 1.3f,
            };
        }

        public void Reset()
        {
            orderTaskRating = 0f;
            waitingTaskRating = 0f;
        }
    }

    [Serializable]
    public class PubTableStateHolder
    {
        public IdlePubTableState idleState;
        public OrderPubTableState orderState;
        public WaitingPubTableState waitingState;
        public ClearPubTableState clearState;

        private PubTableStateHolder()
        {
        }

        public class Builder : IMyBuilder<PubTableStateHolder>
        {
            private PubTableStateHolder holder;
            private PubTableContext context;

            public Builder(PubTableContext context)
            {
                this.context = context;
                holder = new PubTableStateHolder();
            }

            public Builder BuildIdleState()
            {
                holder.idleState = new IdlePubTableState(context);
                return this;
            }

            public Builder BuildOrderState(
                Action onUIInit,
                Action<float> onUIUpdate,
                Action onUIClose
            )
            {
                holder.orderState = new OrderPubTableState(context, onUIInit, onUIUpdate, onUIClose);
                return this;
            }

            public Builder BuildWaitingState(
                Action onUIInit,
                Action<float> onUIUpdate,
                Action onUIClose
            )
            {
                holder.waitingState = new WaitingPubTableState(context, onUIInit, onUIUpdate, onUIClose);
                return this;
            }

            public Builder BuildClearState(Image clearIcon)
            {
                holder.clearState = new ClearPubTableState(context, clearIcon);
                return this;
            }

            public PubTableStateHolder Build()
            {
                return holder;
            }
        }
    }

    public class PubTableContext : MonoBehaviour, IPubTableContext, IItemInteractable,
        IUnsubscribeInteractable<IItemUser>
    {
        #region ContextVariables

        [FoldoutGroup("ContextVariables")] [SerializeField]
        private int typeId;

        [FoldoutGroup("ContextVariables")] [SerializeField]
        private string pubTableName;

        [FoldoutGroup("ContextVariables")] [SerializeReference]
        private PubTableState currentState;

        [field: SerializeField] public string ClientName { get; set; }

        [field: SerializeField] public MoneyTaskRating Rating { get; set; } = new();

        [field: SerializeField] public float TotalOrderItemsPrice { get; set; }

        [FoldoutGroup("ContextVariables")] [SerializeField]
        private PubTableStateHolder holder;

        public int TypeId
        {
            get => typeId;
            set => typeId = value;
        }

        public string Name => pubTableName;

        public PubTableState CurrentState
        {
            get => currentState;
            set => currentState = value;
        }
        public IStorage OrderItems { get; set; }
        public IItemUser CurrentItemUser { get; set; }

        public PubTableStateHolder PubTableStateHolder
        {
            get => holder;
            set => holder = value;
        }

        #endregion

        #region GUIVariables

        [FoldoutGroup("GUIVariables")] [SerializeField] [Required]
        private Image orderProgressBar;

        [FoldoutGroup("GUIVariables")] [SerializeField] [Required]
        private Image waitingProgressBar;

        [FoldoutGroup("GUIVariables")] [SerializeField] [Required]
        private RectTransform servedIconContainer;

        private List<GameObject> servedIconUIs;

        [FoldoutGroup("GUIVariables")] [SerializeField] [Required]
        private GameObject servedIconPrefab;

        [FoldoutGroup("GUIVariables")] [SerializeField]
        private float servedItemSpacing = 0.6f;


        [FoldoutGroup("GUIVariables")] [SerializeField] [Required]
        private Image clearIcon;

        #endregion

        void Awake()
        {
            OrderItems = new StorageEntity(
                $"{gameObject.name}_OrderItems",
                GlobalConfigs.MaxTableSlot
            );

            InitializeUI(GlobalConfigs.MaxTableSlot);

            PubTableStateHolder =
                new PubTableStateHolder.Builder(this)
                    .BuildIdleState()
                    .BuildOrderState(
                        OnOrderStateUIInit,
                        OnOrderStateUIUpdate,
                        OnOrderStateUIClose
                    )
                    .BuildWaitingState(
                        OnWaitingStateUIInit,
                        OnWaitingStateUIUpdate,
                        OnWaitingStateUIClose
                    )
                    .BuildClearState(clearIcon)
                    .Build();
        }

        #region OrderStateUI

        void OnOrderStateUIInit()
        {
            orderProgressBar.gameObject.SetActive(true);
            orderProgressBar.fillAmount = 1f;
        }

        void OnOrderStateUIUpdate(float progress)
        {
            orderProgressBar.fillAmount = progress;
        }

        void OnOrderStateUIClose()
        {
            orderProgressBar.gameObject.SetActive(false);
        }

        #endregion
        
        #region WaitingStateUI

        void OnWaitingStateUIInit()
        {
            waitingProgressBar.gameObject.SetActive(true);
            servedIconContainer.gameObject.SetActive(true);

            waitingProgressBar.fillAmount = 1f;
        }

        void OnWaitingStateUIUpdate(float progress)
        {
            waitingProgressBar.fillAmount = progress;
        }

        void OnWaitingStateUIClose()
        {
            waitingProgressBar.gameObject.SetActive(false);
            servedIconContainer.gameObject.SetActive(false);
        }
        
        #endregion

        #region UIGenerate

        void InitializeUI(int quantity)
        {
            servedIconUIs = new List<GameObject>();
            for (int i = 0; i < quantity; i++)
            {
                GameObject obj = Instantiate(servedIconPrefab, servedIconContainer.transform);
                servedIconUIs.Add(obj);
            }
            //ko xu li link UI - Data. Enable, Disable have executed.

            var horizontalGrid =
                UIMethodHelper.CalculateHorizontalGridPosition(
                    quantity,
                    servedItemSpacing,
                    servedIconPrefab
                );

            int index = 0;
            UIMethodHelper.HandleEnableDisableElements(
                servedIconUIs,
                (obj) =>
                {
                    obj.GetComponent<RectTransform>().anchoredPosition =
                        horizontalGrid.prefabRect.anchoredPosition
                            .With(x: horizontalGrid.firstPostion + horizontalGrid.perComponentSpacing * index);
                    obj.name = $"ServedIcon_{index}";

                    index++;
                }
            );

            servedIconPrefab.SetActive(false);
        }

        #endregion

        public void ChangeState(PubTableState state)
        {
            var oldState = CurrentState;

            CurrentState = state;
            oldState.Exit();
            CurrentState.Entry();
        }

        private void OnEnable()
        {
            PubServiceSystem.Instance.pubTableContexts.Add(this);
            OrderItemSubscribeEvent();
        }

        void Start()
        {
            CurrentState = PubTableStateHolder.idleState;
            CurrentState.Entry();
        }

        private void OnDisable()
        {
            OrderItemUnsubscribeEvent();
            if (PubServiceSystem.Instance != null)
                PubServiceSystem.Instance.pubTableContexts.Remove(this);
        }

        private Dictionary<StorageSlot, GameObject> servedIconsMapping = new();

        private void OrderItemSubscribeEvent()
        {
            if (OrderItems == null)
            {
                Debug.LogError($"{gameObject.name}_OrderItems is null");
                return;
            }

            for (int i = 0; i < OrderItems.StorageSlots.Count; i++)
            {
                StorageSlot slot = OrderItems.StorageSlots[i];
                servedIconsMapping[slot] = servedIconUIs[i];
                slot.OnSlotUpdated += OnOrderSlotUpdated;
            }
        }

        private void OrderItemUnsubscribeEvent()
        {
            if (OrderItems == null)
            {
                Debug.LogError($"{gameObject.name}_OrderItems is null");
                return;
            }

            foreach (var s in OrderItems.StorageSlots)
            {
                s.OnSlotUpdated -= OnOrderSlotUpdated;

                if (servedIconsMapping.ContainsKey(s))
                {
                    servedIconsMapping.Remove(s);
                }
            }
        }

        void OnOrderSlotUpdated(StorageSlot slot)
        {
            GameObject uiSlotObj = servedIconsMapping[slot];

            if (uiSlotObj == null)
            {
                Debug.LogError($"slot is null");
                return;
            }

            if (slot.ItemData && slot.Quantity > 0)
            {
                uiSlotObj.SetActive(true);

                var slotGUI = uiSlotObj.GetComponent<SlotGUI>();

                slotGUI.slotImage.sprite = slot.ItemData.icon;
                slotGUI.slotQuantity.text = $"{slot.Quantity}";
                slotGUI.slotQuantity.enabled = slot.Quantity > 1;
            }
            else
            {
                uiSlotObj.gameObject.SetActive(false);
            }
        }

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
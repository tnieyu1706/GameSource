using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.InputSystem;
using _Project.Scripts.Item;
using AwesomeAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    public class HotBarView : StorageViewAbstract, IInputUIHandler
    {
        [SerializeField, Required] GameObject ItemUserGameObject;
        public IItemUser ItemUser;

        public InputUIReader InputUI { get; set; }
        public static VisualElement HotBarFrame;

        private Dictionary<SlotElementUI, UnityAction> _slotHandlers = new();

        void Awake()
        {
            InputUI = InputUIReader.Instance;
            ItemUser = ItemUserGameObject.GetComponent<IItemUser>();

            if (ItemUser == null)
            {
                Debug.LogError("ItemUserGameObject is null");
            }
        }

        protected override IEnumerator RegistrySlotsUI()
        {
            if (Slots.Length != InputUI.numberHandlers.Length)
            {
                Debug.Log("Hot bar data is different than the number of slots");
                yield break;
            }

            if (ItemUser == null)
            {
                Debug.Log("No init itemUser selected");
                yield break;
            }

            int index = 0;

            foreach (var slot in Slots)
            {
                slot.OnStartDrag += OnPointerDown;

                UnityAction handler = () => OnSlotSelected(slot);
                InputUI.numberHandlers[index++] += handler;

                _slotHandlers[slot] = handler;
            }

            yield return null;

            this.Invoke(() => OnSlotSelected(Slots[0]), 0.01f);

            yield return null;
        }

        protected override void UnRegistrySlotsUI()
        {
            int index = 0;
            foreach (var slot in Slots)
            {
                slot.OnStartDrag -= OnPointerDown;

                if (_slotHandlers.TryGetValue(slot, out var handler))
                {
                    InputUI.numberHandlers[index] -= handler;
                }

                index++;
            }
        }

        private void OnSlotSelected(SlotElementUI selectedSlot)
        {
            ItemUser.SlotSelected = selectedSlot.Slot;
            SetHotBarFramePosition(selectedSlot.worldBound.position);
        }

        public override IEnumerator InitializeView(int size)
        {
            Slots = new SlotElementUI[size];
            root = uiDocument.rootVisualElement.AddClass("root");

            root.styleSheets.Add(this.styleSheet);

            container = root.CreateChild("container");

            var storage = container.CreateChild("storage");

            // storage.CreateChild("storageHeader").Add(new Label(panelName).AddClass("headerLabel"));
            var storageFrame = storage.CreateChild("storageFrame");

            var scrollView = storageFrame.CreateChild(new ScrollView(ScrollViewMode.Vertical), "slotsScroll");
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            slotsContainer = scrollView.contentContainer;
            slotsContainer.AddToClassList("slotsContainer");
            for (int i = 0; i < size; i++)
            {
                var slot = slotsContainer.CreateChild<SlotElementUI>("slot");

                var slotData = storageData.entity.StorageSlots[i];
                slot.Slot = slotData;

                slotData.OnSlotUpdated += (s) => slot.UpdateUI();
                Slots[i] = slot;
            }

            HotBarFrame = root.CreateChild("hotBarFrame");
            HotBarFrame.BringToFront();
            HotBarFrame.pickingMode = PickingMode.Ignore;

            yield return null;
        }

        private static void SetHotBarFramePosition(Vector2 position)
        {
            HotBarFrame.style.left = position.x;
            HotBarFrame.style.top = position.y;
        }
    }
}
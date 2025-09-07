using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    [Serializable]
    public class StorageView : StorageViewAbstract
    {
        [SerializeField] string panelName = "Storage";

        public override IEnumerator InitializeView(int size)
        {
            Slots = new SlotElementUI[size];
            root = uiDocument.rootVisualElement.AddClass("root");

            root.styleSheets.Add(this.styleSheet);

            container = root.CreateChild("container");

            var storage = container.CreateChild("storage");

            storage.CreateChild("storageHeader").Add(new Label(panelName).AddClass("headerLabel"));
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


            yield return null;
        }
    }
}
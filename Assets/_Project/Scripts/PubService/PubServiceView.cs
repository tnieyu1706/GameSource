using System.Collections;
using _Project.Scripts.Storage;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.PubService
{
    public class PubServiceView : StorageViewAbstract
    {
        private Button openPubButton;
        private Button closePubButton;

        protected override IEnumerator SetupView()
        {
            yield return base.SetupView();
            
            Debug.Log("SetupView of PubServiceView On.");

            openPubButton.clicked += HandleOpenPubButtonClicked;
            closePubButton.clicked += HandleClosePubButtonClicked;
            yield return null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            //button event
            openPubButton.clicked -= HandleOpenPubButtonClicked;
            closePubButton.clicked -= HandleClosePubButtonClicked;
            
        }

        void HandleOpenPubButtonClicked()
        {
            PubServiceSystem.Instance.OpenPubService();
        }

        void HandleClosePubButtonClicked()
        {
            Debug.Log("ClosePubService");
            PubServiceSystem.Instance.ClosePubServiceEarly();
        }
        
        public override IEnumerator InitializeView(int size)
        {
            Slots = new SlotElementUI[size];
            root = uiDocument.rootVisualElement.AddClass("root");

            root.styleSheets.Add(this.styleSheet);

            container = root.CreateChild("container");
            container.pickingMode = PickingMode.Position;

            var storage = container.CreateChild("storage");

            storage.CreateChild("storageHeader").Add(new Label("PubService").AddClass("headerLabel"));
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
            
            //button
            var pubServiceGlobal = container.CreateChild("pubServiceGlobal");
            openPubButton = pubServiceGlobal.CreateChild<Button>("openButton", "pubButton");
            closePubButton = pubServiceGlobal.CreateChild<Button>("closeButton", "pubButton");

            yield return null;
        }
    }
}
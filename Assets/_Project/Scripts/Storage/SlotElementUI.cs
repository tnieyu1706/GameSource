using System;
using _Project.Scripts.Item;
using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    [Serializable]
    public class SlotElementUI : VisualElement
    {
        public Image IconImage;
        public Label StackLabel;
        [SerializeField] private StorageSlot slot;
        public Sprite BaseSprite;

        public StorageSlot Slot
        {
            get => slot;
            set
            {
                slot = value;
                UpdateUI();
            }
        }

        public int Index => parent.IndexOf(this);

        public event Action<PointerDownEvent, SlotElementUI> OnStartDrag;

        public SlotElementUI()
        {
            IconImage = this.CreateChild<Image>("slotIcon");
            StackLabel = this.CreateChild<Label>("slotStackLabel");
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (slot.ItemData == null)
            {
                return;
            }

            OnStartDrag?.Invoke(evt, this);

            evt.StopPropagation();
        }

        public void Set(ItemTypeData itemData, int stack = 0)
        {
            this.slot.SetValue(itemData, stack);
        }

        public void UpdateUI()
        {
            BaseSprite = slot.ItemData ? slot.ItemData.icon : null;
            IconImage.image = BaseSprite ? BaseSprite.texture : null;
            StackLabel.text = slot.Quantity > 1 ? slot.Quantity.ToString() : string.Empty;
            StackLabel.visible = slot.Quantity > 1 ? true : false;
        }

        public void Remove()
        {
            this.slot.SetValue(null, 0);
        }
    }
}
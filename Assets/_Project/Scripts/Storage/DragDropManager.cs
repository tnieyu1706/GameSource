using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.General.Patterns.Singleton;
using _Project.Scripts.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    public interface IUIMangerIntializer
    {
        void InitializeView(VisualElement root);
    }
    
    public class DragDropManager : BehaviorSingleton<DragDropManager>, IUIMangerIntializer
    {
        public readonly List<IDragView> DragViews = new List<IDragView>();
        public static bool IsDragging = false;
        public static SlotElementUI OriginalSlot;
        public static VisualElement GhostIcon;
        [SerializeField] protected StyleSheet styleSheet;

        public Action<SlotElementUI, SlotElementUI> OnDrop;

        private void OnEnable()
        {
            OnDrop += OnDropAction;
        }

        private void OnDisable()
        {
            OnDrop -= OnDropAction;
        }

        public void InitializeView(VisualElement root)
        {
            GhostIcon = root.CreateChild("ghostIcon");
            GhostIcon.BringToFront();

            GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (!IsDragging) return;

            SlotElementUI closestSlot = null;
            foreach (var storage in DragViews)
            {
                if (storage.DragSlots != null && GhostIcon.worldBound.Overlaps(storage.DraggedElementHitBox.worldBound))
                {
                    closestSlot = storage.DragSlots
                        .Where(slot => slot.worldBound.Overlaps(GhostIcon.worldBound))
                        .OrderBy(slot => Vector2.Distance(slot.worldBound.position, GhostIcon.worldBound.position))
                        .FirstOrDefault();
                    break;
                }
            }

            if (closestSlot != null)
            {
                OnDrop?.Invoke(OriginalSlot, closestSlot);
            }
            else
            {
                OriginalSlot.Set(
                    OriginalSlot.Slot.ItemData,
                    OriginalSlot.Slot.Quantity
                );
            }

            IsDragging = false;
            OriginalSlot = null;

            GhostIcon.style.visibility = Visibility.Hidden;
            evt.StopPropagation();
            GhostIcon.ReleasePointer(evt.pointerId);

        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!IsDragging) return;
            SetGhostIconPosition(evt.position);
        }

        public static void SetGhostIconPosition(Vector2 position)
        {
            GhostIcon.style.top = position.y - GhostIcon.layout.height / 2;
            GhostIcon.style.left = position.x - GhostIcon.layout.width / 2;
        }

        static void OnDropAction(SlotElementUI originSlot, SlotElementUI droppedSlot)
        {
            ItemTypeData originItem = originSlot.Slot.ItemData;
            int originStack = originSlot.Slot.Quantity;

            if (droppedSlot.Slot.ItemData == null)
            {
                droppedSlot.Set(originItem, originStack);
                originSlot.Remove();
            }
            else if (droppedSlot == originSlot)
            {
                droppedSlot.Set(originItem, originStack);
            }
            else if (droppedSlot.Slot.ItemData == originSlot.Slot.ItemData)
            {
                originStack += droppedSlot.Slot.Quantity;
                droppedSlot.Set(originItem, originStack);
                originSlot.Remove();
            }
            else
            {
                ItemTypeData droppedItem = droppedSlot.Slot.ItemData;
                int droppedStack = droppedSlot.Slot.Quantity;

                droppedSlot.Set(originItem, originStack);
                originSlot.Set(droppedItem, droppedStack);
            }
        }

        [Button("Check")]
        void CheckDragViews()
        {
            Debug.Log($"Dragviews count : {DragViews.Count}");
        }
    }
}
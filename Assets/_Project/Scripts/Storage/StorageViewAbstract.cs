using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    public interface IDragView
    {
        List<SlotElementUI> DragSlots { get; }
        VisualElement DraggedElementHitBox { get; }
    }

    public abstract class StorageViewAbstract : MonoBehaviour, IDragView
    {
        [SerializeField] public StorageSO storageData;
        public SlotElementUI[] Slots;

        [SerializeField] protected UIDocument uiDocument;
        [SerializeField] protected StyleSheet styleSheet;

        protected VisualElement root;
        protected VisualElement container;
        public VisualElement slotsContainer;

        public List<SlotElementUI> DragSlots => Slots.ToList();
        public VisualElement DraggedElementHitBox => slotsContainer;

        IEnumerator OnStartView()
        {
            if (storageData == null)
            {
                Debug.Log("StorageData is null");
                yield break;
            }
            else
            {
                yield return InitializeView(storageData.entity.SlotQuantity);
            }
        }

        protected virtual IEnumerator RegistrySlotsUI()
        {
            foreach (var slot in Slots)
            {
                slot.OnStartDrag += OnPointerDown;
            }

            yield return null;
        }

        protected virtual void UnRegistrySlotsUI()
        {
            foreach (var slot in Slots)
            {
                slot.OnStartDrag -= OnPointerDown;
            }
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(SetupView());

            DragDropManager.Instance.DragViews.Add(this);
        }

        protected virtual IEnumerator SetupView()
        {
            yield return OnStartView();
            yield return RegistrySlotsUI();
        }

        protected virtual void OnDisable()
        {
            UnRegistrySlotsUI();

            if (DragDropManager.Instance != null)
                DragDropManager.Instance.DragViews.Remove(this);
        }

        protected virtual void OnPointerDown(PointerDownEvent evt, SlotElementUI slotUI)
        {
            if (evt.button == 0)
            {
                DragDropManager.IsDragging = true;
                DragDropManager.OriginalSlot = slotUI;

                DragDropManager.SetGhostIconPosition(evt.position);

                DragDropManager.GhostIcon.style.backgroundImage = slotUI.BaseSprite.texture;
                slotUI.IconImage.image = null;
                slotUI.StackLabel.visible = false;

                // UIManager.Instance.GhostIcon.style.opacity = 0.8f;
                DragDropManager.GhostIcon.style.visibility = Visibility.Visible;

                DragDropManager.GhostIcon.CapturePointer(evt.pointerId);
            }
        }

        public abstract IEnumerator InitializeView(int size);
    }
}
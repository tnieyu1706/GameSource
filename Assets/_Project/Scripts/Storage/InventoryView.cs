using _Project.Scripts.InputSystem;
using _Project.Scripts.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.Storage
{
    public class InventoryView : StorageView
    {
        protected override void OnPointerDown(PointerDownEvent evt, SlotElementUI slotUI)
        {
            base.OnPointerDown(evt, slotUI);

            if (evt.button == 1)
            {
                //Drop...
            }
        }

    }
}
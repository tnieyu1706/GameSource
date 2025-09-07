using System;
using _Project.Scripts.Item;

namespace _Project.Scripts.Interact
{
    public interface IItemInteractor : IInteractor
    {
        ItemTypeData SelectedItem { get; }
        Action OnSelected { get; set; }
        Action OnDeselected { get; set; }
        Action<ItemTypeData> OnUsing { get; set; }
    }
}
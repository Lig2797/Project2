using System;
using UnityEngine;

public class InventoryEvents
{
    public event Action<string> onItemAdded;
    public void AddItem(string itemName)
    {
        onItemAdded?.Invoke(itemName);
    }

    public event Action<InventoryItem, int> onAddItemToUI;
    public void AddItemToUI(InventoryItem invenItem, int index)
    {
        onAddItemToUI?.Invoke(invenItem, index);
    }
}

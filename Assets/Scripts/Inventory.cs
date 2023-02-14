using System;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public int stackCapacity { get; private set; } // Number of stacks the inventory can hold. This is NOT
    // the same as the quantity of items it can hold.
    public List<ItemQuantity> stacks { get; private set; }

    private void Awake()
    {
        stacks = new(stackCapacity);
    }

    /**
     * Returns the index in the inventory where the item occurs.
     * -1 indicates it is not in the inventory.
     */
    public int GetIndexInInventory(Item item)
    {
        return InventoryUtils.GetIndexInInventory(item, stacks);
    }

    public bool ContainsItem(Item item)
    {
        return InventoryUtils.ContainsItem(item, stacks);
    }

    public void Add(ItemQuantity iq)
    {
        InventoryUtils.Add(iq, stacks);
    }

    /**
     * If strict is true, an error will be thrown if iq.quantity > the existing quantity.
     * If strict is false, no error will be thrown and the item will be removed from inv
     */
    public void Remove(ItemQuantity iq, bool strict)
    {
        InventoryUtils.Remove(iq, stacks, strict);
    }

    public void Remove(ItemQuantity iq) {
        InventoryUtils.Remove(iq, stacks, true);
    }

    public bool HasAtLeast(ItemQuantity iq)
    {
        return InventoryUtils.HasAtLeast(iq, stacks);
    }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message): base(message)
    {

    }
}

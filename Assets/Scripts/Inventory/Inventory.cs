using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public int StackCapacity { get; private set; } // Number of stacks the inventory can hold. This is NOT
    // the same as the quantity of items it can hold.
    public List<ItemQuantity> Stacks { get; private set; }

    public Inventory(int stackCapacity)
    {
        StackCapacity = stackCapacity;
        Stacks = new(StackCapacity);
    }

    public void Initialize(int stackCapacity)
    {
        StackCapacity = stackCapacity;
        Stacks = new(StackCapacity);
    }

    /**
     * Returns the index in the inventory where the item occurs.
     * -1 indicates it is not in the inventory.
     */
    public int GetIndexInInventory(Item item)
    {
        return InventoryUtils.GetIndexInInventory(item, Stacks);
    }

    public bool ContainsItem(Item item)
    {
        return InventoryUtils.ContainsItem(item, Stacks);
    }

    public void Add(ItemQuantity iq, bool strict)
    {
        InventoryUtils.Add(iq, Stacks, strict);
    }

    public void Add(ItemQuantity iq)
    {
        InventoryUtils.Add(iq, Stacks, true);
    }

    /**
     * If strict is true, an error will be thrown if iq.quantity > the existing quantity.
     * If strict is false, no error will be thrown and the item will be removed from inv
     */
    public void Remove(ItemQuantity iq, bool strict)
    {
        InventoryUtils.Remove(iq, Stacks, strict);
    }

    public void Remove(ItemQuantity iq) {
        InventoryUtils.Remove(iq, Stacks, true);
    }

    public bool HasAtLeast(ItemQuantity iq)
    {
        return InventoryUtils.HasAtLeast(iq, Stacks);
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append("[");
        foreach (ItemQuantity iq in Stacks)
        {
            builder.Append(iq.ToString());
            builder.Append(" ");
        }
        builder.Append("]");
        return builder.ToString();
    }

    /**
     * Used primarily as a callback supporting the UI.
     * This will make all the changes to the inventory listed out
     * in changedIndices.
     * 
     * Assume changedIndices[0] is the inventory index changed,
     * and changedIndices[1] is the new value for that index.
     * 
     * Example of how this is used:
     * - Player rearranges items in their inventory using the UI.
     * - The UI records each of those changes to the cells, mapping
     * them to indices in the inventory list.
     * - The UI triggers this callback. The player's changes in the
     * inventory UI are now effected in the backend as well.
     */
    public void MakeChanges(List<(int, ItemQuantity)> changedIndices)
    {
        foreach ((int, ItemQuantity) tuple in changedIndices)
        {
            Stacks[tuple.Item1] = tuple.Item2;
        }
    }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message): base(message) { }
}

public class InventoryFullException : Exception
{
    public InventoryFullException(string message) : base(message) { }
}
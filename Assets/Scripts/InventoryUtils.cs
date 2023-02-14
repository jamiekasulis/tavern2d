using System.Collections.Generic;
using UnityEngine;

/**
 * Holds all the generic list logic for Inventory so that
 * this functionality can be unit testable.
 */
public class InventoryUtils
{
    public static int GetIndexInInventory(Item item, List<ItemQuantity> inv)
    {
        return inv.FindIndex((iq) => iq.item.Equals(item));
    }

    public static bool ContainsItem(Item item, List<ItemQuantity> inv)
    {
        return GetIndexInInventory(item, inv) >= 0;
    }

    public static void Add(ItemQuantity iq, List<ItemQuantity> inv)
    {
        int idx = GetIndexInInventory(iq.item, inv);
        if (idx < 0)
        {
            if (inv.Capacity == inv.Count)
            {
                // No more space to add a new stack
                throw new InventoryFullException("Cannot add new stack to inventory. It is full!");
            }

            inv.Add(iq);
        }
        else
        {
            ItemQuantity updated = inv[idx];
            updated.quantity += iq.quantity;
            inv[idx] = updated;
        }
    }

    public static void Remove(ItemQuantity iq, List<ItemQuantity> inv, bool strict)
    {
        int idx = GetIndexInInventory(iq.item, inv);
        if (idx < 0)
        {
            string msg = "Requested to remove " + iq.ToString() + " but item is not present in inventory";
            if (strict)
            {
                throw (new InvalidQuantityException(msg));
            }
            else {
                Debug.LogWarning(msg);
                return;
            }
        }

        int newQty = inv[idx].quantity - iq.quantity;
        if (newQty < 0)
        {
            string msg = "Requested to remove " + iq.quantity + " of " + iq.item.itemName + " but there is only " + inv[idx].quantity;
            if (strict)
            {
                throw (new InvalidQuantityException(msg));
            }
            else
            {
                Debug.LogWarning(msg);
                inv.RemoveAt(idx);
            }
        }
        else if (newQty == 0)
        {
            inv.RemoveAt(idx);
        }
        else
        {
            inv[idx].quantity = newQty;
        }
    }

    public static bool HasAtLeast(ItemQuantity iq, List<ItemQuantity> inv)
    {
        int idx = GetIndexInInventory(iq.item, inv);
        if (idx < 0)
        {
            return false;
        }
        return inv[idx].quantity >= iq.quantity;
    }
}

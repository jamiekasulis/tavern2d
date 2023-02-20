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
        return inv.FindIndex((iq) => iq != null && iq.item.Equals(item));
    }

    public static bool ContainsItem(Item item, List<ItemQuantity> inv)
    {
        return GetIndexInInventory(item, inv) >= 0;
    }

    public static void Add(ItemQuantity iq, List<ItemQuantity> inv, bool strict)
    {
        int idx = GetIndexInInventory(iq.item, inv);
        if (idx < 0)
        {
            if (HasEmptySpace(inv))
            {
                inv[FirstEmptyIndex(inv)] = iq;
            } else
            {
                // No more space to add a new stack
                string msg = "Cannot add new stack " + iq.ToString() + " to inventory. It is full!";
                if (strict)
                {
                    throw new InventoryFullException(msg);
                }
                else
                {
                    Debug.Log(msg);
                    return;
                }
            }
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
                inv[idx] = null;
            }
        }
        else if (newQty == 0)
        {
            inv[idx] = null;
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

    private static int FirstEmptyIndex(List<ItemQuantity> inv)
    {
        return inv.FindIndex(iq => iq == null);
    }

    public static int Size(List<ItemQuantity?> inv)
    {
        return inv.FindAll(i => i != null).Count;
    }

    public static bool HasEmptySpace(List<ItemQuantity> inv)
    {
        bool result = Size(inv) < inv.Capacity;
        Debug.Log($"HasRoom: True size={Size(inv)}, capacity={inv.Capacity}. HasRoom={result}");
        return result;
    }
}

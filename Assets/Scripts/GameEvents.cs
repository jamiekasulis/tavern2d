using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent<ItemPickedUpArgs> ItemPickedUp;

    public void OnItemPickedUp(ItemPickedUpArgs args)
    {
        ItemPickedUp.Invoke(args);
    }
}

public class ItemPickedUpArgs : EventArgs
{
    public ItemQuantity iq { get; private set; }
    public Inventory targetInventory { get; private set; }
}

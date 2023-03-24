using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent<ItemPickedUpArgs> ItemPickedUp;
    [SerializeField] private UnityEvent BuildModeEnabled;
    [SerializeField] private UnityEvent BuildModeDisabled;

    public void OnItemPickedUp(ItemPickedUpArgs args)
    {
        ItemPickedUp.Invoke(args);
    }

    public void OnBuildModeEnabled()
    {
        BuildModeEnabled.Invoke();
    }

    
}

public class ItemPickedUpArgs : EventArgs
{
    public ItemQuantity iq { get; private set; }
    public Inventory targetInventory { get; private set; }
}

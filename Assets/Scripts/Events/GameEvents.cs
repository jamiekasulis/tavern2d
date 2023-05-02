using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * This is mostly an experiment. But I want to try out having one file contain 
 * the event functions and then reference these from wherever events are being 
 * triggered.
 */
public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] public UnityEvent<ItemPickedUpArgs> ItemPickedUp;
    [SerializeField] public UnityEvent BuildModeEnabled;
    [SerializeField] public UnityEvent BuildModeDisabled;
    [SerializeField] public UnityEvent TestEvent;

    public void OnItemPickedUp(ItemPickedUpArgs args)
    {
        ItemPickedUp.Invoke(args);
    }

    public void OnBuildModeEnabled()
    {
        BuildModeEnabled.Invoke();
    }

    public void OnBuildModeDisabled()
    {
        BuildModeDisabled.Invoke();
    }


}

public class ItemPickedUpArgs : EventArgs
{
    public ItemQuantity iq { get; private set; }
    public Inventory targetInventory { get; private set; }
}

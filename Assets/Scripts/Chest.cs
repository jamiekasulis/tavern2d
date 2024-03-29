using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : Interactable
{
    // This is just for testing. In the future, when I dig into how I want to
    // design loading custom save data and world data, I should get rid of this.
    [SerializeField] List<ItemQuantity> testInventoryContents;
    private Inventory inventory;
    private string chestName = "Chest";
    public bool IsOpen { get; private set; }
    [SerializeField] private UnityEvent<Inventory> OpenInventoryTrigger;
    [SerializeField] private UnityEvent<Inventory> CloseInventoryTrigger;

    private void Awake()
    {
        IsOpen = false;
        inventory = new(20);
        foreach (var iq in testInventoryContents)
        {
            inventory.Add(iq);
        }
    }

    public void Open()
    {
        IsOpen = true;
        OpenInventoryTrigger.Invoke(inventory);
    }

    public void Close()
    {
        IsOpen = false;
        CloseInventoryTrigger.Invoke(inventory);
    }

    public void Take()
    {
        // @TODO
    }

    public void Put()
    {
        // @TODO
    }

    public void Organize()
    {
        // Consolidate stacks and push all filled spots to the beginning of the
        // inventory.
        // @TODO
    }

    override public void Interact()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}

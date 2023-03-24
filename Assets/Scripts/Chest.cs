using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : Interactable
{
    // Need a better way to designate the items in a chest. This is just for testing.
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
        Debug.Log("Opened Chest: " + inventory.ToString());

        IsOpen = true;
        OpenInventoryTrigger.Invoke(inventory);
    }

    public void Close()
    {
        Debug.Log("Closed chest.");
        IsOpen = false;
        CloseInventoryTrigger.Invoke(inventory);
    }

    public void Take()
    {

    }

    public void Put()
    {

    }

    public void Organize()
    {
        // Consolidate stacks and push all filled spots to the beginning of the
        // inventory.
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

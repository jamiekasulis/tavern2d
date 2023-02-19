using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    // Need a better way to designate the items in a chest. This is just for testing.
    [SerializeField] List<Item> itemsInChest;
    private Inventory inventory;

    public bool isOpen { get; private set; }

    private void Awake()
    {
        isOpen = false;
        inventory = new(20);
        foreach (Item i in itemsInChest)
        {
            inventory.Add(new() { item = i, quantity = 5 });
        }
    }

    public void Open()
    {
        isOpen = true;

        Debug.Log("Opened Chest: " + inventory.ToString());
    }

    public void Close()
    {
        isOpen = false;
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
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}

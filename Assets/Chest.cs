using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Inventory inventory;

    private void Awake()
    {
        inventory.Initialize(30);
    }

    public void Open()
    {
        
    }

    public void Close()
    {

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
}

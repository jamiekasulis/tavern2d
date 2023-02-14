using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickUp : MonoBehaviour
{
    public bool automaticPickup; // If true, item will be automatically picked up
    // if the player inventory has space.
    [SerializeField] public ItemQuantity itemQuantity;

    public PickUp()
    {
        InventoryManager.instance.PlayerInventory.Add()
    }
}

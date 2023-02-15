using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Collider2D))]
public class PickUp : MonoBehaviour
{
    public bool automaticPickup; // If true, item will be automatically picked up
    // if the player inventory has space.
    public ItemQuantity itemQuantity;

    public PickUp(ItemQuantity itemQuantity, bool automaticPickup)
    {
        this.itemQuantity = itemQuantity;
        this.automaticPickup = automaticPickup;
    }

    public void AddToPlayerInventory()
    {
        if (automaticPickup)
        {
            AutomaticPickup();
        }
        else
        {
            ManualPickup();
        }
    }

    private void AutomaticPickup()
    {
        InventoryManager.instance.PlayerInventory.Add(itemQuantity, false);
        Destroy(this.gameObject); // @TODO Graceful despawn by vacuuming the item in & playing a sound effect
    }

    private void ManualPickup()
    {
        InventoryManager.instance.PlayerInventory.Add(itemQuantity, false);
        Destroy(this.gameObject); // @TODO Play a sound effect
    }
}

using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public PlayerInventoryMenu PlayerInventoryMenu;
    public Inventory PlayerInventory { get; private set; }
    // The current inventory to consider. This is usually the player inventory,
    // but if a chest is open or we are in a crafting room then this can change.
    public Inventory ActiveInventory { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        gameObject.AddComponent<Inventory>().Initialize(10);
        PlayerInventory = gameObject.GetComponent<Inventory>();
        PlayerInventory.Initialize(10);
        ActiveInventory = PlayerInventory;
    }

    public void TogglePlayerInventoryMenuEnabled()
    {
        PlayerInventoryMenu.ToggleMenuOpen();
    }

    public void PickupToPlayerInventory(PickUp pickup)
    {
        void pickupAndDestroy(PickUp pickup)
        {
            PlayerInventory.Add(pickup.itemQuantity);
            Destroy(pickup.gameObject, 0);
        }

        Debug.Log("Invoked PickupToPlayerInventory with pickup = " + pickup.itemQuantity.item.itemName);
        int idx = PlayerInventory.GetIndexInInventory(pickup.itemQuantity.item);
        if (idx < 0)
        {
            // Check if there's space in player inventory
            if (PlayerInventory.Stacks.Count < PlayerInventory.Stacks.Capacity)
            {
                pickupAndDestroy(pickup);
            }
            else
            {
                // There is no room
                Debug.Log("Cannot pick up item. No room in backpack!");
            }
        }
        else
        {
            // Currently there is no limit on each stack's quantity, so this is fine
            pickupAndDestroy(pickup);
        }
    }
}

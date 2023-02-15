using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Inventory PlayerInventory { get; private set; }
    // The current inventory to consider. This is usually the player inventory,
    // but if a chest is open or we are in a crafting room then this can change.
    public Inventory ActiveInventory { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        gameObject.AddComponent<Inventory>().Initialize(10);
        PlayerInventory = gameObject.GetComponent<Inventory>();
        PlayerInventory.Initialize(10);
        ActiveInventory = PlayerInventory;
    }
}

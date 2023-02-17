using UnityEngine;
using UnityEngine.UIElements;

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
        PlayerInventoryMenu.ToggleEnabled();
    }
}

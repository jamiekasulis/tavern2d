using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [SerializeField] public InventoryMenu2 PlayerInventoryMenu;
    [SerializeField] private UnityEvent<Inventory> redrawInventoryTrigger;
    [SerializeField] private UnityEvent<List<(Item?, VisualElement)>> buildModeToggledTrigger;

    public bool ingestTestPlayerInventory = false;

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
        DontDestroyOnLoad(gameObject);
    }

    public void TogglePlayerInventoryMenuEnabled()
    {
        PlayerInventoryMenu.ToggleMenuOpen(GameState.Instance.Player.Inventory);
    }

    public void PickupToPlayerInventory(PickUp pickup)
    {
        Inventory playerInventory = GameState.Instance.Player.Inventory;

        void handlePickup(PickUp pickup)
        {
            playerInventory.Add(pickup.itemQuantity);
            Destroy(pickup.gameObject, 0);
            redrawInventoryTrigger.Invoke(playerInventory);
        }

        int idx = playerInventory.GetIndexInInventory(pickup.itemQuantity.item);
        if (idx < 0)
        {
            // Check if there's space in player inventory
            if (playerInventory.HasEmptySpace())
            {
                handlePickup(pickup);
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
            handlePickup(pickup);
        }
    }

    public void MakeChangesToInventory(Inventory inv, List<(int, ItemQuantity)> changedIndices)
    {
        inv.MakeChanges(changedIndices);
    }
}

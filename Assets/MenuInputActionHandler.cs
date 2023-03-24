using UnityEngine;

public class MenuInputActionHandler : MonoBehaviour
{
    public void OnToggleInventoryMenu()
    {
        InventoryManager.Instance.TogglePlayerInventoryMenuEnabled();
    }
}

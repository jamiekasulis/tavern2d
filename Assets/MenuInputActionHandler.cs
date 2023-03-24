using UnityEngine;

public class MenuInputActionHandler : MonoBehaviour
{
    public void OnToggleInventoryMenu()
    {
        InventoryManager.Instance.TogglePlayerInventoryMenuEnabled();
    }

    public void OnToggleBuildMode()
    {
        BuildMode.Instance.ToggleBuildMode();
    }

    public void OnRotateLeft()
    {
        if (BuildMode.Instance.IsEnabled)
        {
            BuildMode.Instance.RotateObject(PlaceableObject.RotationDirectionEnum.Left);
        }
    }

    public void OnRotateRight()
    {
        if (BuildMode.Instance.IsEnabled)
        {
            BuildMode.Instance.RotateObject(PlaceableObject.RotationDirectionEnum.Right);
        }
    }
}

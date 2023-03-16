using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * This class contains the build mode-related functionailities of the inventory
 * menu.
 */
public class BuildModeInventoryMenuTriggers : MonoBehaviour
{

    /**
     * This should be the callback that triggers the UI to reflect the changes
     * made by these functions.
     */
    [SerializeField] private UnityEvent<List<CellData2>> RedrawInventoryMenuCells;

    /**
     * Invoke me when build mode is enabled/disabled
     */
    public void ApplyBuildModeStylingToInventory()
    {
        Debug.Log($"Invoked ApplyBuildModeStylingToInventory. BuildModeEnabled={BuildMode.Instance.IsEnabled}");
        // @TODO Change this to PLAYER Inventory Menu, not CHEST!
        CellData2[,] cellData = InventoryManager.Instance.ChestInventoryMenu.cellsByRow;

        if (BuildMode.Instance.IsEnabled)
        {
            // Apply styles
            foreach (CellData2 cell in cellData)
            {
                if (cell.itemData != null && cell.itemData.item.buildMode)
                {
                    Debug.Log($"Applying OK styling on cell with {cell.itemData.item.itemName}");
                    cell.additionalStyles.Add(CellData2.InventoryCellStyleEnum.BuildModeOK);
                }
                else if (cell.itemData != null && !cell.itemData.item.buildMode)
                {
                    Debug.Log($"Applying NOT_OK styling on cell with {cell.itemData.item.itemName}");
                    cell.additionalStyles.Add(CellData2.InventoryCellStyleEnum.BuildModeNotOK);
                }
                else
                {
                    // Do nothing if the cell has no item in it.
                }
            }
        }
        else
        {
            // Remove build mode-related styles
            foreach (CellData2 cell in cellData)
            {
                cell.additionalStyles.Remove(CellData2.InventoryCellStyleEnum.BuildModeOK);
                cell.additionalStyles.Remove(CellData2.InventoryCellStyleEnum.BuildModeNotOK);
            }
        }
        List<CellData2> flatList = ListUtils.FlattenToList(cellData);
        Debug.Log($"Flattened list: {flatList.Count}");
        RedrawInventoryMenuCells.Invoke(flatList);
    }

    public void OnCellClick()
    {
        // @TODO implement me!
    }
}

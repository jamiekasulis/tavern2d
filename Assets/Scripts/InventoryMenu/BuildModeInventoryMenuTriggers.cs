using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * This class extracts the callbacks which coordinate between different
 * components (like inventory and UI) and build mode.
 */
public class BuildModeInventoryMenuTriggers : MonoBehaviour
{

    /**
     * This should be the callback that triggers the UI to reflect the changes
     * made by these functions.
     */
    [SerializeField] private UnityEvent<List<CellData>> RedrawInventoryMenuCells;
    /**
     * This callback is responsible for making sure the Inventoy backend is updated
     * with the correct inventory data.
     */
    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> ReflectChangesToInventoryBackendCallback;

    /**
     * Invoke me when build mode is enabled/disabled.
     * This is responsible for updating the CellData in the InventoryMenu,
     * specifically the additionalStyles property to attach the correct build 
     * mode-related styling.
     * 
     * Then this ensures any impacted cells are redrawn.
     */
    public void ApplyBuildModeStylingToInventory()
    {
        CellData[,] cellData = InventoryManager.Instance.PlayerInventoryMenu.cellsByRow;

        List<CellData> updatedCells = new List<CellData>();
        if (BuildMode.Instance.IsEnabled)
        {
            // Apply styles
            
            foreach (CellData cell in cellData)
            {
                if (cell.itemData != null && cell.itemData.item.buildMode)
                {
                    cell.additionalStyles.Add(CellData.InventoryCellStyleEnum.BuildModeOK);
                    updatedCells.Add(cell);
                }
                else if (cell.itemData != null && !cell.itemData.item.buildMode)
                {
                    cell.additionalStyles.Add(CellData.InventoryCellStyleEnum.BuildModeNotOK);
                    updatedCells.Add(cell);
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
            foreach (CellData cell in cellData)
            {
                cell.additionalStyles.Remove(CellData.InventoryCellStyleEnum.BuildModeOK);
                cell.additionalStyles.Remove(CellData.InventoryCellStyleEnum.BuildModeNotOK);
            }
        }
        RedrawInventoryMenuCells.Invoke(updatedCells);
    }

    public void SelectBuildModeObjectCallback(CellData cell)
    {
        Debug.Log("Invoked SelectBuildModeObjectCallback");
        // Invoke callbacks to set the objectToPlace in build mode and
        // reflect the changes in inventory (backend & UI redraw)
        if (cell.itemData != null && cell.itemData.item.buildMode)
        {
            // Set the object that will be placed in Build Mode
            BuildMode.Instance.SetObjectToPlace(cell.itemData.item);

            // Decrement qty=1 of that item from the inventory backend
            ItemQuantity updated = new()
            {
                item = cell.itemData.item,
                quantity = cell.itemData.quantity - 1
            };
            
            int inventoryArrayIndex = InventoryMenu.GridToInventoryIndex(
                cell.row, cell.col,
                InventoryManager.Instance.PlayerInventoryMenu.GridSizeSpecification
            );

            ReflectChangesToInventoryBackendCallback.Invoke(
                GameState.Instance.Player.Inventory,
                new List<(int, ItemQuantity)>
                {
                    (inventoryArrayIndex, updated.quantity > 0 ? updated : null)
                }
            );

            // Redraw the affected UI cell
            cell.itemData = updated.quantity > 0 ? updated : null;
            RedrawInventoryMenuCells.Invoke(new List<CellData>() { cell });
        }
        else
        {
            Debug.LogWarning($"SelectBuildModeObjectCallback was called, but " +
                $"for a cell containing a non-build mode-approved item " +
                $"({cell.itemData}). Will do nothing.");
            return;
        }
        

    }

    public void OnCellClick()
    {
        // @TODO implement me!
    }
}

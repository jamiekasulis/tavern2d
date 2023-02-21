using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(GridSizeSpecification))]
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;
    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> rearrangeTrigger;

    private CellData? selectedCell = null;
    private Inventory inventory;


    GridSizeSpecification gridSize;


    // UI Elements
    private VisualElement root;
    private ScrollView GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    private CellData[,] cellsByRow; // We assume these to be using InventoryCell.uxml

    class CellData
    {
        public VisualElement visualElement;
        public ItemQuantity? itemData;
        public int row, col;

        public CellData(VisualElement cellVisualElement, ItemQuantity? itemData, int row, int column)
        {
            visualElement = cellVisualElement;
            this.itemData = itemData;
            this.row = row;
            col = column;
        }
    }

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<ScrollView>("GridContainerScrollView");
        GridContainer.Clear(); // Do this since Awake() gets called 2x sometimes?
        title = root.Q<Label>("Title");

        gridSize = gameObject.GetComponent<GridSizeSpecification>();

        /* Create the grid of cells.
         * These are just some GridRow templates holding
         * InventoryCell templates.
         */
        rows = new VisualElement[gridSize.GetNumRows()];
        cellsByRow = new CellData[gridSize.GetNumRows(), gridSize.GetNumCols()];

        // Compose the 2D array of cells mapped to rows
        // Create GridRows
        for (int r = 0; r < gridSize.GetNumRows(); r++)
        {
            rows[r] = GridRowTemplate.Instantiate();
            GridContainer.contentContainer.Add(rows[r]);
        }

        // Populate GridRows with InventoryCells
        for(int r = 0; r < gridSize.GetNumRows(); r++)
        {
            for (int c = 0; c < gridSize.GetNumCols(); c++)
            {
                CellData cell = new(CellTemplate.Instantiate(), null, r, c);
                cell.visualElement.RegisterCallback<MouseDownEvent, CellData>(HandleCellClick, cell, useTrickleDown: TrickleDown.TrickleDown);

                cellsByRow[r, c] = cell;
                rows[r].Q<IMGUIContainer>("Row").Add(cellsByRow[r,c].visualElement);
            }
        }

        root.style.display = DisplayStyle.None;
        root.SetEnabled(false);
    }

    private void OnEnable()
    {

    }

    /**
     * Draws the inventory. We assume that the UIDocument already contains 
     * a set of empty InventoryCell templates. This iterates over the inventory
     * and replaces the data in each pre-existing cell to match what's stored
     * at that index in the inventory.
     */
    public void DrawInventory(Inventory inventory)
    {
        this.inventory = inventory;

        title.text = MenuTitle;

        // Fill in each cell. This requires mapping from 1-dimensional
        // indices in inventory to 2-dimensional indices in cellsByRow.
        for (int i = 0; i < inventory.StackCapacity; i++)
        {
            (int, int) rowCol = InventoryToGridIndex(i);

            ItemQuantity? maybeItem = inventory.Stacks[i];
            if (maybeItem != null)
            {
                DrawCell(rowCol.Item1, rowCol.Item2, inventory.Stacks[i]);
            }
            else
            {
                EmptyCell(rowCol.Item1, rowCol.Item2, false);
            }
        }
    }

    private (int, int) InventoryToGridIndex(int inventoryIndex)
    {
        return (inventoryIndex / gridSize.GetNumCols(), inventoryIndex % gridSize.GetNumCols());
    }

    private int GridToInventoryIndex(int row, int col)
    {
        int result = gridSize.GetNumCols() * row + col;
        return result;
    }

    private void DrawCell(int row, int col, ItemQuantity itemData)
    {
        CellData cell = cellsByRow[row, col];
        Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
        Button rootButton = cell.visualElement.Q<Button>("RootButton");

        cell.itemData = itemData;
        qtyLabel.text = itemData.quantity.ToString();
        rootButton.style.backgroundImage = new StyleBackground(itemData.item.sprite);
        rootButton.style.unityBackgroundImageTintColor = StyleKeyword.Null;
    }

    private void EmptyCell(int row, int col, bool greyOut)
    {
        CellData cell = cellsByRow[row, col];
        Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
        Button rootButton = cell.visualElement.Q<Button>("RootButton");

        if (greyOut)
        {
            rootButton.style.unityBackgroundImageTintColor = Color.gray;
        }
        else
        {
            qtyLabel.text = "";
            rootButton.style.backgroundImage = StyleKeyword.None;
        }
        
        cell.itemData = null;
    }

    /**
    * Toggles both the enabled status and visibility of the menu.
    */
    public void ToggleMenuOpen(Inventory inventory)
    {
        bool newEnabledValue = !root.enabledInHierarchy;
        root.SetEnabled(newEnabledValue);
        StyleEnum<DisplayStyle> oldDisplayStyle = root.style.display;
        root.style.display = oldDisplayStyle == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;

        if (newEnabledValue)
        {
            DrawInventory(inventory);
        }
    }

    private void HandleCellClick(MouseDownEvent evt, CellData cell)
    {
        bool isHoldingItem = selectedCell != null;
        bool cellHasItem = cell.itemData != null;

        // Tracks which indices of the INVENTORY object were changed,
        // with the item quantity being what to change it to.
        // This will be passed to a callback at the end which will
        // reflect the visual changes in the UI to the actual Inv data.
        List<(int, ItemQuantity)> changedIndices = new();

        // No-op
        if (!isHoldingItem && !cellHasItem)
        {
            return;
        }

        // Picking up an item
        else if (!isHoldingItem && cellHasItem)
        {
            if (evt.button == 2) // Middle mouse button click - Do nothing
            {
                return;
            }

            if (IsLeftClickOnly(evt)) // Left click: select whole stack
            {
                PickUpWholeStack(cell, changedIndices);
            }
            else if (IsShiftLeftClick(evt)) // Shift+Left click: select half the stack (round up)
            {
                int pickupQty = cell.itemData.quantity % 2 == 0 ? cell.itemData.quantity / 2 : cell.itemData.quantity / 2 + 1;
                PickUpQuantity(cell, pickupQty, changedIndices);
            }
            else if (IsRightClick(evt)) // Right click: select 1
            {
                PickUpQuantity(cell, 1, changedIndices);
            }
        }

        // Placing an item
        // Subcase: Swap
        else if (isHoldingItem && cellHasItem)
        {
            CellData temp = new(cell.visualElement, cell.itemData, cell.row, cell.col);
            DrawCell(cell.row, cell.col, selectedCell.itemData);
            DrawCell(selectedCell.row, selectedCell.col, temp.itemData);

            changedIndices.Add((
                GridToInventoryIndex(temp.row, temp.col),
                selectedCell.itemData
            ));
            changedIndices.Add((
                GridToInventoryIndex(selectedCell.row, selectedCell.col),
                temp.itemData
            ));

            selectedCell = null;
        }

        // Subcase: Place into empty slot
        else if (isHoldingItem && !cellHasItem)
        {
            if (cell.row == selectedCell.row && cell.col == selectedCell.col)
            {
                // Has not moved
                DrawCell(cell.row, cell.col, selectedCell.itemData);
            } else
            {
                DrawCell(cell.row, cell.col, selectedCell.itemData);
                EmptyCell(selectedCell.row, selectedCell.col, false);
            }

            changedIndices.Add((
                GridToInventoryIndex(cell.row, cell.col),
                selectedCell.itemData
            ));
            selectedCell = null;
        }
        else
        {
            return;
        }

        rearrangeTrigger.Invoke(inventory, changedIndices);
    }

    private void PickUpQuantity(CellData cell, int qtyToPickUp, List<(int, ItemQuantity?)> changedIndices)
    {
        if (cell.itemData.quantity < qtyToPickUp)
        {
            Debug.LogWarning($"Requested to pick up {qtyToPickUp} from cell ({cell.row},{cell.col}) but there is only {cell.itemData.quantity} available!");
            return;
        }
        else if (qtyToPickUp <= 0)
        {
            Debug.LogWarning($"Requested to pick up an invalid quantity: {qtyToPickUp}");
            return;
        }


        int qtyLeftBehind = cell.itemData.quantity - qtyToPickUp;
        if (qtyLeftBehind == 0)
        {
            PickUpWholeStack(cell, changedIndices);
            return;
        }
        selectedCell = new CellData(cell.visualElement, new() { item = cell.itemData.item, quantity = qtyToPickUp }, cell.row, cell.col);
        ItemQuantity updatedIq = new() { item = cell.itemData.item, quantity = qtyLeftBehind };
        DrawCell(cell.row, cell.col, updatedIq);
        changedIndices.Add((
            GridToInventoryIndex(cell.row, cell.col),
            updatedIq
        ));
    }

    private void PickUpWholeStack(CellData cell, List<(int, ItemQuantity?)> changedIndices)
    {
        selectedCell = new CellData(cell.visualElement, cell.itemData, cell.row, cell.col);
        EmptyCell(cell.row, cell.col, true);
        changedIndices.Add((
            GridToInventoryIndex(cell.row, cell.col),
            null
        ));
        Debug.Log($"Picked up " + selectedCell.itemData.ToString());
    }

    //private void PickUpHalfStack(CellData cell, List<(int, ItemQuantity?)> changedIndices)
    //{
    //    if (cell.itemData.quantity == 1)
    //    {
    //        PickUpWholeStack(cell, changedIndices);
    //        return;
    //    }
    //    // Pick up half, rounded up
    //    int amountPickedUp = cell.itemData.quantity % 2 == 0 ? cell.itemData.quantity / 2 : cell.itemData.quantity / 2 + 1;
    //    int amountLeftBehind = cell.itemData.quantity - amountPickedUp;

    //    selectedCell = new CellData(cell.visualElement, new() { item = cell.itemData.item, quantity = amountPickedUp }, cell.row, cell.col);
    //    ItemQuantity updatedIq = new() { item = cell.itemData.item, quantity = amountLeftBehind };
    //    DrawCell(cell.row, cell.col, updatedIq);
    //    changedIndices.Add((
    //        GridToInventoryIndex(cell.row, cell.col),
    //        updatedIq
    //    ));
    //    Debug.Log($"Picked up " + selectedCell.itemData.ToString());
    //}

    //private void PickUpOne(CellData cell, List<(int, ItemQuantity?)> changedIndices)
    //{
    //    if (cell.itemData.quantity == 1)
    //    {
    //        PickUpWholeStack(cell, changedIndices);
    //        return;
    //    }

    //    int amountLeftBehind = cell.itemData.quantity - 1;

    //    selectedCell = new CellData(cell.visualElement, new() { item = cell.itemData.item, quantity = 1 }, cell.row, cell.col);
    //    ItemQuantity updatedIq = new() { item = cell.itemData.item, quantity = amountLeftBehind };
    //    DrawCell(cell.row, cell.col, updatedIq);
    //    changedIndices.Add((
    //        GridToInventoryIndex(cell.row, cell.col),
    //        updatedIq
    //    ));
    //    Debug.Log($"Picked up " + selectedCell.itemData.ToString());
    //}

    private bool IsLeftClickOnly(MouseDownEvent evt)
    {
        return evt.button == 0 && !evt.shiftKey;
    }

    private bool IsShiftLeftClick(MouseDownEvent evt)
    {
        return evt.button == 0 && evt.shiftKey;
    }

    private bool IsRightClick(MouseDownEvent evt)
    {
        return evt.button == 1;
    }
}

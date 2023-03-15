using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(GridSizeSpecification))]
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> rearrangeInventoryTrigger;
    [SerializeField] private UnityEvent<Item?, VisualElement> buildModeDrawCellTrigger;

    public RearrangeInventoryTooltip inventoryTooltip;

    private CellData? selectedCell = null;
    private Inventory inventory;

    public static Color HighlightColor { get; private set; } = new Color(255, 215, 25);

    GridSizeSpecification gridSize;


    // UI Elements
    private VisualElement root;
    private ScrollView GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    public CellData[,] cellsByRow { get; private set; } // We assume these to be using InventoryCell.uxml

    public class CellData
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

    public bool IsOpen()
    {
        return root.style.display != DisplayStyle.None &&
            root.enabledInHierarchy;
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
            VisualElement e;
            if (maybeItem != null)
            {
                e = DrawCell(rowCol.Item1, rowCol.Item2, inventory.Stacks[i]);
            }
            else
            {
                e = EmptyCell(rowCol.Item1, rowCol.Item2);
            }

            buildModeDrawCellTrigger.Invoke(maybeItem?.item, e);
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

    /**
     * Draws the given item data into the cell at the specified coordinates.
     */
    private VisualElement DrawCell(int row, int col, ItemQuantity itemData)
    {
        CellData cell = cellsByRow[row, col];
        Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
        Button rootButton = cell.visualElement.Q<Button>("RootButton");

        cell.itemData = itemData;
        qtyLabel.text = itemData.quantity.ToString();
        rootButton.style.backgroundImage = new StyleBackground(itemData.item.spriteFront);
        rootButton.style.unityBackgroundImageTintColor = StyleKeyword.Null;
        return rootButton;
    }

    /**
     * Empties out the cell so that there is no image sprite or label shown.
     */
    private VisualElement EmptyCell(int row, int col)
    {
        CellData cell = cellsByRow[row, col];
        Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
        Button rootButton = cell.visualElement.Q<Button>("RootButton");

        qtyLabel.text = "";
        rootButton.style.backgroundImage = StyleKeyword.None;
        
        cell.itemData = null;
        return rootButton;
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

    /**
     * Event callback for mouse activity. Handles the user rearranging inventory
     * in the UI.
     */
    private void HandleCellClick(MouseDownEvent evt, CellData cell)
    {
        bool isHoldingItem = selectedCell != null;
        bool cellHasItem = cell.itemData != null;

        // Tracks which indices of the INVENTORY object were changed,
        // with the item quantity being what to change it to.
        // This will be passed to a callback at the end which will
        // reflect the visual changes in the UI to the backend inventory object.
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

            if (IsShiftLeftClick(evt)) // Shift+Left click: select half the stack (round up)
            {
                int pickupQty = cell.itemData.quantity % 2 == 0 ? cell.itemData.quantity / 2 : cell.itemData.quantity / 2 + 1;
                changedIndices = PickUpQuantity(cell, pickupQty);
            }
            else if (IsLeftClickOnly(evt)) // Left click: select whole stack
            {
                changedIndices = PickUpQuantity(cell, cell.itemData.quantity);
            }
            else if (IsRightClick(evt)) // Right click: select 1
            {
                changedIndices = PickUpQuantity(cell, 1);
            }
        }

        // Placing an item
        // Subcase: Swapping and combining
        else if (isHoldingItem && cellHasItem)
        {
            if (IsLeftClickOnly(evt))
            {
                changedIndices = PlaceIntoOccupiedSlot(cell, selectedCell.itemData.quantity);
            }
        }

        // Subcase: Place into empty slot
        else if (isHoldingItem && !cellHasItem)
        {
            int qtyToPlace;
            if (IsShiftLeftClick(evt))
            {
                qtyToPlace = selectedCell.itemData.quantity % 2 == 0 ? selectedCell.itemData.quantity / 2 : selectedCell.itemData.quantity / 2 + 1;
            }
            else if (IsLeftClickOnly(evt))
            {
                qtyToPlace = selectedCell.itemData.quantity;
            }
            else if (IsRightClick(evt))
            {
                qtyToPlace = 1;
            }
            else
            {
                return;
            }

            changedIndices = PlaceIntoEmptySlot(cell, qtyToPlace);
        }

        if (selectedCell == null)
        {
            inventoryTooltip.Clear();
        }
        else
        {
            inventoryTooltip.Draw(selectedCell.itemData);
        }

        if (changedIndices.Count > 0)
        {
            rearrangeInventoryTrigger.Invoke(inventory, changedIndices);
        }
    }

    private List<(int, ItemQuantity?)> PickUpQuantity(CellData cell, int qtyToPickUp)
    {
        if (cell.itemData.quantity < qtyToPickUp)
        {
            throw new InvalidQuantityException($"Requested to pick up {qtyToPickUp} from cell ({cell.row},{cell.col}) but there is only {cell.itemData.quantity} available!");
        }
        else if (qtyToPickUp <= 0)
        {
            throw new InvalidQuantityException($"Requested to pick up an invalid quantity: {qtyToPickUp}");
        }

        ItemQuantity selectedIq = new() { item = cell.itemData.item, quantity = qtyToPickUp };
        selectedCell = new CellData(cell.visualElement, selectedIq, cell.row, cell.col);

        int qtyLeftBehind = cell.itemData.quantity - qtyToPickUp;
        ItemQuantity updatedIq = new() { item = cell.itemData.item, quantity = qtyLeftBehind };
        if (qtyLeftBehind == 0)
        {
            EmptyCell(cell.row, cell.col);
        }
        else
        {
            DrawCell(cell.row, cell.col, updatedIq);
        }

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            qtyLeftBehind == 0 ? null : updatedIq)
        };
    }

    private List<(int, ItemQuantity?)> PlaceIntoEmptySlot(CellData cell, int qtyToPlace)
    {
        if (cell.itemData != null)
        {
            throw new CellNotEmptyException($"Requested to place into empty cell, but given cell ({cell.row},{cell.col}) is not empty!");
        }

        // Place into the clicked cell
        ItemQuantity iqToPlace = new() { item = selectedCell.itemData.item, quantity = qtyToPlace };
        DrawCell(cell.row, cell.col, iqToPlace);
        selectedCell.itemData.quantity -= qtyToPlace;

        // Check if the selectedCell is all used up/placed
        if (selectedCell.itemData.quantity <= 0)
        {
            selectedCell = null;
        }

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            iqToPlace)
        };
    }

    private List<(int, ItemQuantity?)> PlaceIntoOccupiedSlot(CellData cell, int qtyToPlace)
    {
        bool sameItem = selectedCell.itemData.item == cell.itemData.item;
        ItemQuantity? newIq;
        if (sameItem)
        {
            newIq = new() { item = cell.itemData.item, quantity = cell.itemData.quantity + qtyToPlace };
            DrawCell(cell.row, cell.col, newIq);
            selectedCell = null;
        }
        else // Swapping
        {
            // @TODO Do this without holding onto references that will then not be garbage-collected.
            newIq = selectedCell.itemData;
            CellData temp = new(cell.visualElement, cell.itemData, cell.row, cell.col);
            DrawCell(cell.row, cell.col, selectedCell.itemData);
            selectedCell.row = cell.row;
            selectedCell.col = cell.col;
            selectedCell.itemData = temp.itemData;
            selectedCell.visualElement = temp.visualElement;
        }

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            newIq)
        };
    }

    private bool IsLeftClickOnly(MouseDownEvent evt)
    {
        // Do not use evt.shiftKey as from my testing this is inaccurate 1/2 the time
        return evt.button == 0 && !Input.GetKey(KeyCode.LeftShift);
    }

    private bool IsShiftLeftClick(MouseDownEvent evt)
    {
        // Do not use evt.shiftKey as from my testing this is inaccurate 1/2 the time
        return evt.button == 0 && Input.GetKey(KeyCode.LeftShift);
    }

    private bool IsRightClick(MouseDownEvent evt)
    {
        return evt.button == 1;
    }

    public List<(Item?, VisualElement)> GetItemToVisualElementMapping()
    {
        List<(Item?, VisualElement)> data = new(cellsByRow.Length);
        foreach (var cellData in cellsByRow)
        {
            data.Add((
                cellData.itemData?.item,
                cellData.visualElement
            ));
        }
        return data;
    }
}

public class CellNotEmptyException : Exception
{
    public CellNotEmptyException(string message) : base(message) { }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private StyleSheet BuildModeOKStyleSheet;
    [SerializeField] private StyleSheet BuildModeNotOKStyleSheet;
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    public GridSizeSpecification GridSizeSpecification { get; private set; }
    public RearrangeInventoryTooltip inventoryTooltip;

    #region Callbacks

    /**
     * Callback responsible for reflecting changes made to the data rendered in the Inventory Menu to the
     * backend Inventory representation
     */
    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> reflectChangesToBackendInventoryCallback;
    /**
     * Callback responsible for handling selecting an item from inventory
     * that is to be placed during build mode.
     * This should set the objectToPlace in build mode AND remove
     * the item from the inventory.
     */
    [SerializeField] private UnityEvent<CellData> SelectBuildModeObjectCallback;

    /**
     * Handles updating the inventory rearrangement tooltip.
     */
    [SerializeField] private UnityEvent<ItemQuantity?> UpdatedTooltipCallback;

    #endregion

    // Private unserializeds
    // UI Elements
    private VisualElement root;
    private ScrollView GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    public CellData[,] cellsByRow { get; private set; } // We assume these to be using InventoryCell.uxml
    private ItemQuantity? itemInHand = null;
    private Inventory inventory;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<ScrollView>("GridContainerScrollView");
        GridContainer.Clear(); // Do this since Awake() gets called 2x sometimes?
        title = root.Q<Label>("Title");

        GridSizeSpecification = gameObject.GetComponent<GridSizeSpecification>();

        /* Create the grid of cells.
         * These are just some GridRow templates holding
         * InventoryCell templates.
         */
        rows = new VisualElement[GridSizeSpecification.GetNumRows()];
        cellsByRow = new CellData[GridSizeSpecification.GetNumRows(), GridSizeSpecification.GetNumCols()];

        // Compose the 2D array of cells mapped to rows
        // Create GridRows
        for (int r = 0; r < GridSizeSpecification.GetNumRows(); r++)
        {
            rows[r] = GridRowTemplate.Instantiate();
            GridContainer.contentContainer.Add(rows[r]);
        }

        // Populate GridRows with InventoryCells
        for (int r = 0; r < GridSizeSpecification.GetNumRows(); r++)
        {
            for (int c = 0; c < GridSizeSpecification.GetNumCols(); c++)
            {
                CellData cell = new(CellTemplate.Instantiate(), null, r, c);
                cell.visualElement.RegisterCallback<MouseDownEvent, CellData>(HandleCellClick, cell, useTrickleDown: TrickleDown.TrickleDown);

                cellsByRow[r, c] = cell;
                rows[r].Q<IMGUIContainer>("Row").Add(cellsByRow[r, c].visualElement);
            }
        }

        root.style.display = DisplayStyle.None;
        root.SetEnabled(false);
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

    #region Draw functions

    /**
     * Draws the given inventory data into the inventory menu.
     * Uses default styling and properties for all slots.
     */
    public void DrawInventory(Inventory inv)
    {
        inventory = inv;
        title.text = MenuTitle;

        // Fill in each cell, which requires mapping indices from 1-dimensional
        // inventory to 2-dimensional cell data.
        for (int i = 0; i < inv.StackCapacity; i++)
        {
            (int, int) rowCol = InventoryToGridIndex(i);
            ItemQuantity? maybeItem = inventory.Stacks[i];
            CellData cellDataUpdated = cellsByRow[rowCol.Item1, rowCol.Item2];

            cellDataUpdated.itemData = maybeItem;
            RedrawCells(new List<CellData>() { cellDataUpdated });
        }
    }

    /**
     * Updates the visuals of each cell according to the cell data passed in.
     * Will first set the cell to its default styling, then apply specialized
     * styling from CellData.additionalStyles on top of that.
     */
    public void RedrawCells(List<CellData> cellsToRedraw)
    {
        cellsToRedraw.ForEach(cell =>
        {
            cell.visualElement.styleSheets.Clear();
            Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
            Button rootButton = cell.visualElement.Q<Button>("RootButton");

            qtyLabel.text = cell.itemData?.quantity.ToString() ?? "";
            rootButton.style.backgroundImage =
                cell.itemData != null
                    ? new StyleBackground(cell.itemData.item.spriteFront)
                    : StyleKeyword.None;

            // Remove any styling if a cell is empty
            if (cell.itemData == null || cell.itemData?.quantity <= 0) // <=0 shouldn't happen, but just in case
            {
                cell.additionalStyles.Clear();
            }

            if (cell.additionalStyles.Count > 0)
            {
                LoadAdditionalStyles(cell);
            }

            //cellsByRow[cell.row, cell.col] = cell; // Do this so that the UI is holding onto the correct
        });
    }

    /**
     * Attaches all of the additional style sheet(s) to the cell.
     * Which styles get attached is determined by the cell's additionalStyles
     * property.
     */
    private void LoadAdditionalStyles(CellData cell)
    {
        foreach (CellData.InventoryCellStyleEnum style in cell.additionalStyles)
        {
            StyleSheet? styleSheet =
                style == CellData.InventoryCellStyleEnum.BuildModeOK ? BuildModeOKStyleSheet
                : style == CellData.InventoryCellStyleEnum.BuildModeNotOK ? BuildModeNotOKStyleSheet
                : null;

            if (styleSheet == null)
            {
                continue;
            }
            cell.visualElement.styleSheets.Add(styleSheet);
        }
    }

    #endregion

    private void HandleCellClick(MouseDownEvent evt, CellData cell)
    {
        bool isHoldingItem = itemInHand != null;
        bool cellHasItem = cell.itemData != null;

        // Check if we are selecting an object to place while in build mode
        if (BuildMode.Instance.IsEnabled)
        {
            if (cell.itemData != null &&
                cell.itemData.item.buildMode)
            {
                if (isHoldingItem)
                {
                    // Shouldn't happen but just in case
                    throw new System.Exception($"Somehow we are in build mode, clicking an item, while still holding some items to rearrange. Will do nothing.");
                }

                if (!cellHasItem)
                {
                    // do nothing
                }
                else
                {
                    SelectBuildModeObjectCallback.Invoke(cell);
                }
            }
        }

        // No-op
        if (!isHoldingItem && !cellHasItem)
        {
            return;
        }

        // Tracks which indices of the INVENTORY object were changed,
        // with the item quantity being what to change it to.
        // This will be passed to a callback at the end which will
        // reflect the visual changes in the UI to the backend inventory object.
        List<(int, ItemQuantity)> changedIndices = new();

        // Picking up an item
        if (!isHoldingItem && cellHasItem)
        {
            changedIndices = HandleItemPickup(evt, cell);
        }
        // Placing an item
        // Subcase: Swapping and combining
        else if (isHoldingItem && cellHasItem)
        {
            changedIndices = HandleSwapOrCombine(evt, cell);
        }

        // Subcase: Place into empty slot
        else if (isHoldingItem && !cellHasItem)
        {
            changedIndices = HandleItemPlacement(evt, cell);
        }

        UpdatedTooltipCallback.Invoke(itemInHand);

        if (changedIndices.Count > 0)
        {
            reflectChangesToBackendInventoryCallback.Invoke(inventory, changedIndices);
        }
    }

    #region Helper functions

    private (int, int) InventoryToGridIndex(int inventoryIndex)
    {
        return (inventoryIndex / GridSizeSpecification.GetNumCols(), inventoryIndex % GridSizeSpecification.GetNumCols());
    }

    private int GridToInventoryIndex(int row, int col)
    {
        int result = GridSizeSpecification.GetNumCols() * row + col;
        return result;
    }

    public static int GridToInventoryIndex(int row, int col, GridSizeSpecification gridSizeSpecification)
    {
        int result = gridSizeSpecification.GetNumCols() * row + col;
        return result;
    }

    private List<(int, ItemQuantity)> HandleItemPickup(MouseDownEvent evt, CellData cell)
    {
        int pickupQty;
        if (MouseUtils.IsShiftLeftClick(evt)) // Shift+Left click: select half the stack (round up)
        {
            pickupQty = cell.itemData.quantity % 2 == 0 ? cell.itemData.quantity / 2 : cell.itemData.quantity / 2 + 1;
        }
        else if (MouseUtils.IsLeftClickOnly(evt)) // Left click: select whole stack
        {
            pickupQty = cell.itemData.quantity;
        }
        else if (MouseUtils.IsRightClick(evt)) // Right click: select 1
        {
            pickupQty = 1;
        }
        else
        {
            return new();
        }

        return PickUpQuantity(cell, pickupQty);
    }

    private List<(int, ItemQuantity)> HandleSwapOrCombine(MouseDownEvent evt, CellData cell)
    {
        if (MouseUtils.IsLeftClickOnly(evt))
        {
            return PlaceIntoOccupiedSlot(cell, itemInHand.quantity);
        }
        else
        {
            return new();
        }
    }

    private List<(int, ItemQuantity)> HandleItemPlacement(MouseDownEvent evt, CellData cell)
    {
        int qtyToPlace;
        if (MouseUtils.IsShiftLeftClick(evt))
        {
            qtyToPlace = itemInHand.quantity % 2 == 0 ? itemInHand.quantity / 2 : itemInHand.quantity / 2 + 1;
        }
        else if (MouseUtils.IsLeftClickOnly(evt))
        {
            qtyToPlace = itemInHand.quantity;
        }
        else if (MouseUtils.IsRightClick(evt))
        {
            qtyToPlace = 1;
        }
        else
        {
            return new();
        }

        return PlaceIntoEmptySlot(cell, qtyToPlace);
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

        itemInHand = new ItemQuantity() { item = cell.itemData.item, quantity = qtyToPickUp };

        int qtyLeftBehind = cell.itemData.quantity - qtyToPickUp;
        cell.itemData.quantity = qtyLeftBehind;

        if (qtyLeftBehind == 0)
        {
            cell.itemData = null;
        }
        else
        {
            cell.itemData.quantity = qtyLeftBehind;
        }

        RedrawCells(new List<CellData> { cell });

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            qtyLeftBehind == 0 ? null : cell.itemData)
        };
    }

    private List<(int, ItemQuantity?)> PlaceIntoOccupiedSlot(CellData cell, int qtyToPlace)
    {
        bool sameItem = itemInHand.item == cell.itemData.item;

        if (sameItem) // Combining
        {
            cell.itemData = new() { item = cell.itemData.item, quantity = cell.itemData.quantity + qtyToPlace };
            RedrawCells(new List<CellData> { cell });

            itemInHand.quantity -= qtyToPlace;
            if (itemInHand.quantity <= 0)
            {
                itemInHand = null;
            }
        }
        else // Swapping
        {
            ItemQuantity temp = cell.itemData;
            cell.itemData = new ItemQuantity() { item = itemInHand.item, quantity = itemInHand.quantity };
            RedrawCells(new List<CellData> { cell });

            itemInHand = new ItemQuantity() { item = temp.item, quantity = temp.quantity };
        }

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            cell.itemData)
        };
    }

    private List<(int, ItemQuantity?)> PlaceIntoEmptySlot(CellData cell, int qtyToPlace)
    {
        if (cell.itemData != null)
        {
            throw new System.Exception($"Requested to place into empty cell, but given cell ({cell.row},{cell.col}) is not empty!");
        }
        if (itemInHand == null)
        {
            throw new System.Exception($"Requested to place into empty cell, but player is not holding any item to place!");
        }

        // Place into the clicked cell
        cell.itemData = new() { item = itemInHand.item, quantity = qtyToPlace };
        RedrawCells(new List<CellData> { cell });
        itemInHand.quantity -= qtyToPlace;
        if (itemInHand.quantity <= 0)
        {
            itemInHand = null;
        }

        return new()
        {
            (GridToInventoryIndex(cell.row, cell.col),
            cell.itemData)
        };
    }

    #endregion

    public bool IsOpen()
    {
        return root.style.display != DisplayStyle.None &&
            root.enabledInHierarchy;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class CellData2
{
    public VisualElement visualElement;
    public ItemQuantity? itemData;
    public int row, col;
    public bool buttonEnabled;
    public List<InventoryCellStyleEnum> additionalStyles;

    public enum InventoryCellStyleEnum
    {
        BuildModeOK = 0,
        BuildModeNotOK = 1
    }

    public CellData2(VisualElement cellVisualElement, ItemQuantity? itemData, int row, int column)
    {
        visualElement = cellVisualElement;
        this.itemData = itemData;
        this.row = row;
        col = column;
        buttonEnabled = true;
        additionalStyles = new List<InventoryCellStyleEnum>();
    }

    public CellData2(VisualElement cellVisualElement, ItemQuantity? itemData, int row, int column, List<InventoryCellStyleEnum> additionalStyles)
    {
        visualElement = cellVisualElement;
        this.itemData = itemData;
        this.row = row;
        col = column;
        buttonEnabled = true;
        this.additionalStyles = additionalStyles;
    }
}

public class InventoryMenu2 : MonoBehaviour
{
    [SerializeField] private StyleSheet BuildModeOKStyleSheet;
    [SerializeField] private StyleSheet BuildModeNotOKStyleSheet;
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    public GridSizeSpecification GridSizeSpecification { get; private set; }

    // Callbacks
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
    [SerializeField] private UnityEvent<CellData2> SelectBuildModeObjectCallback;

    // Private unserializeds
    // UI Elements
    private VisualElement root;
    private ScrollView GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    public CellData2[,] cellsByRow { get; private set; } // We assume these to be using InventoryCell.uxml
    private CellData2? selectedCell = null;
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
        cellsByRow = new CellData2[GridSizeSpecification.GetNumRows(), GridSizeSpecification.GetNumCols()];

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
                CellData2 cell = new(CellTemplate.Instantiate(), null, r, c);
                cell.visualElement.RegisterCallback<MouseDownEvent, CellData2>(HandleCellClick, cell, useTrickleDown: TrickleDown.TrickleDown);

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
            CellData2 cellDataUpdated = cellsByRow[rowCol.Item1, rowCol.Item2];

            cellDataUpdated.itemData = maybeItem;
            RedrawCells(new List<CellData2>() { cellDataUpdated });
        }
    }

    /**
     * Updates the visuals of each cell according to the cell data passed in.
     * Will first set the cell to its default styling, then apply specialized
     * styling from CellData.additionalStyles on top of that.
     */
    public void RedrawCells(List<CellData2> cellsToRedraw)
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

            if (cell.additionalStyles.Count > 0)
            {
                LoadAdditionalStyles(cell);
            }
        });
    }

    /**
     * Attaches all of the additional style sheet(s) to the cell.
     * Which styles get attached is determined by the cell's additionalStyles
     * property.
     */
    private void LoadAdditionalStyles(CellData2 cell)
    {
        foreach (CellData2.InventoryCellStyleEnum style in cell.additionalStyles)
        {
            StyleSheet? styleSheet =
                style == CellData2.InventoryCellStyleEnum.BuildModeOK ? BuildModeOKStyleSheet
                : style == CellData2.InventoryCellStyleEnum.BuildModeNotOK ? BuildModeNotOKStyleSheet
                : null;

            if (styleSheet == null)
            {
                Debug.Log($"Did not find stylesheet corresponding to {style}. Skipping");
                continue;
            }
            Debug.Log($"Applying stylesheet for {style} to cell ({cell.row},{cell.col})");
            cell.visualElement.styleSheets.Add(styleSheet);
        }
    }

    #endregion

    private void HandleCellClick(MouseDownEvent evt, CellData2 cell)
    {
        bool isHoldingItem = selectedCell != null;
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
    }

    #region Helper functions

    private (int, int) InventoryToGridIndex(int inventoryIndex)
    {
        return (inventoryIndex / GridSizeSpecification.GetNumCols(), inventoryIndex % GridSizeSpecification.GetNumCols());
    }

    // @TODO Move me to an InventoryMenuUtils class
    public static int GridToInventoryIndex(int row, int col, GridSizeSpecification gridSizeSpecification)
    {
        int result = gridSizeSpecification.GetNumCols() * row + col;
        return result;
    }

    #endregion

    public bool IsOpen()
    {
        return root.style.display != DisplayStyle.None &&
            root.enabledInHierarchy;
    }
}

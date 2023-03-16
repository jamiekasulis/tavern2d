using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class InventoryMenu2 : MonoBehaviour
{
    [SerializeField] private StyleSheet BuildModeOKStyleSheet;
    [SerializeField] private StyleSheet BuildModeNotOKStyleSheet;
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    // Callbacks
    /**
     * Callback responsible for reflecting changes made to the data rendered in the Inventory Menu to the
     * backend Inventory representation
     */
    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> reflectChangesToBackendInventoryCallback;

    // Private unserializeds
    GridSizeSpecification gridSize;
    // UI Elements
    private VisualElement root;
    private ScrollView GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    public CellData2[,] cellsByRow { get; private set; } // We assume these to be using InventoryCell.uxml
    private CellData2? selectedCell = null;
    private Inventory inventory;
    private IStyle defaultCellStyle;

    public enum InventoryCellStyleEnum
    {
        BUILD_MODE_STYLE = 0
    }

    public class CellData2
    {
        public VisualElement visualElement;
        public ItemQuantity? itemData;
        public int row, col;
        public bool buttonEnabled;
        public List<InventoryCellStyleEnum> additionalStyles;

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
        cellsByRow = new CellData2[gridSize.GetNumRows(), gridSize.GetNumCols()];

        // Compose the 2D array of cells mapped to rows
        // Create GridRows
        for (int r = 0; r < gridSize.GetNumRows(); r++)
        {
            rows[r] = GridRowTemplate.Instantiate();
            GridContainer.contentContainer.Add(rows[r]);
        }

        // Populate GridRows with InventoryCells
        for (int r = 0; r < gridSize.GetNumRows(); r++)
        {
            for (int c = 0; c < gridSize.GetNumCols(); c++)
            {
                CellData2 cell = new(CellTemplate.Instantiate(), null, r, c);
                //cell.visualElement.RegisterCallback<MouseDownEvent, CellData2>(HandleCellClick, cell, useTrickleDown: TrickleDown.TrickleDown);

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

    /**
     * Draws the given inventory data into the inventory menu.
     * Uses default styling and properties for all slots.
     */
    public void DrawInventory(Inventory inv)
    {
        Debug.Log($"InventoryMenu2.DrawInventory");

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
     */
    public void RedrawCells(List<CellData2> cellsToRedraw)
    {
        Debug.Log($"InventoryMenu2.RedrawCells");

        cellsToRedraw.ForEach(cell =>
        {
            Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
            Button rootButton = cell.visualElement.Q<Button>("RootButton");

            qtyLabel.text = cell.itemData?.quantity.ToString() ?? "";
            rootButton.style.backgroundImage =
                cell.itemData != null
                    ? new StyleBackground(cell.itemData.item.spriteFront)
                    : StyleKeyword.None;
            // @TODO Move this to the right spot later. Just testing
            cell.visualElement.styleSheets.Add(BuildModeOKStyleSheet);
        });
    }

    #region Helper functions

    private (int, int) InventoryToGridIndex(int inventoryIndex)
    {
        return (inventoryIndex / gridSize.GetNumCols(), inventoryIndex % gridSize.GetNumCols());
    }

    private int GridToInventoryIndex(int row, int col)
    {
        int result = gridSize.GetNumCols() * row + col;
        return result;
    }

    #endregion
}

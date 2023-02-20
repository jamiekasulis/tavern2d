using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

[RequireComponent(typeof(GridSizeSpecification))]
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;
    private CellData? selectedCell = null;

    // Grid size - See doc on GridSizeSpecification class for why I
    // do it this way
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
        title.text = MenuTitle;

        // Fill in each cell. This requires mapping from 1-dimensional
        // indices in inventory to 2-dimensional indices in cellsByRow.
        for (int i = 0; i < inventory.Stacks.Capacity; i++)
        {
            int col = i % gridSize.GetNumCols();
            int row = i / gridSize.GetNumCols();

            if (i < inventory.Stacks.Count)
            {
                DrawCell(row, col, inventory.Stacks[i]);
            }
            else
            {
                EmptyCell(row, col, false);
            }
        }
    }

    private void DrawCell(int row, int col, ItemQuantity itemData)
    {
        CellData cell = cellsByRow[row, col];
        Label qtyLabel = cell.visualElement.Q<Label>("QuantityLabel");
        Button rootButton = cell.visualElement.Q<Button>("RootButton");

        cell.itemData = itemData;
        qtyLabel.text = itemData.quantity.ToString();
        rootButton.style.backgroundImage = new StyleBackground(itemData.item.sprite);
        //rootButton.style.unityBackgroundImageTintColor = Color.clear;
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

        // No-op
        if (!isHoldingItem && !cellHasItem)
        {
            return;
        }

        // Picking up an item
        else if (!isHoldingItem && cellHasItem)
        {
            selectedCell = new CellData(cell.visualElement, cell.itemData, cell.row, cell.col);
            EmptyCell(cell.row, cell.col, true);
        }

        // Placing an item
        // Subcase: Swap
        else if (isHoldingItem && cellHasItem)
        {
            CellData temp = new(cell.visualElement, cell.itemData, cell.row, cell.col);
            DrawCell(cell.row, cell.col, selectedCell.itemData);
            DrawCell(selectedCell.row, selectedCell.col, temp.itemData);
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
            selectedCell = null;
        }
    }
}

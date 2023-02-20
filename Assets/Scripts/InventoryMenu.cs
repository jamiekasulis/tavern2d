using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(GridSizeSpecification))]
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    // Grid size - See doc on GridSizeSpecification class for why I
    // do it this way
    GridSizeSpecification gridSize;


    // UI Elements
    private VisualElement root;
    private IMGUIContainer GridContainer;
    private Label title;
    private VisualElement[] rows; // GridRows
    private VisualElement[,] cellsByRow; // We assume these to be using InventoryCell.uxml

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");
        GridContainer.Clear(); // Do this since Awake() gets called 2x sometimes?
        title = root.Q<Label>("Title");

        gridSize = gameObject.GetComponent<GridSizeSpecification>();

        /* Create the grid of cells.
         * These are just some GridRow templates holding
         * InventoryCell templates.
         */
        rows = new VisualElement[gridSize.GetNumRows()];
        cellsByRow = new VisualElement[gridSize.GetNumRows(), gridSize.GetNumCols()];
        for (int r = 0; r < gridSize.GetNumRows(); r++)
        {
            rows[r] = GridRowTemplate.Instantiate();
            GridContainer.contentContainer.Add(rows[r]);
        }

        // Compose the 2D array of cells mapped to rows
        for(int r = 0; r < gridSize.GetNumRows(); r++)
        {
            for (int c = 0; c < gridSize.GetNumCols(); c++)
            {
                cellsByRow[r, c] = CellTemplate.Instantiate();
                //rows[r].contentContainer.Add(cellsByRow[r,c]);
                rows[r].Q<IMGUIContainer>("Row").Add(cellsByRow[r, c]);
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
        Debug.Log("Trying to draw inventory. " + inventory.ToString());
        title.text = MenuTitle;

        // Fill in each cell. This requires mapping from 1-dimensional
        // indices in inventory to 2-dimensional indices in cellsByRow.
        for (int i = 0; i < inventory.Stacks.Capacity; i++)
        {
            int col = i % gridSize.GetNumCols();
            int row = i / gridSize.GetNumCols();

            VisualElement cell = cellsByRow[row, col];
            Label qtyLabel = cell.Q<Label>("QuantityLabel");
            Button rootButton = cell.Q<Button>("RootButton");
            if (i < inventory.Stacks.Count)
            {
                // Draw in the item info
                ItemQuantity iq = inventory.Stacks[i];
                Debug.Log($"Filling in item info for ({row},{col}). item={iq}");
                qtyLabel.text = iq.quantity.ToString();
                rootButton.style.backgroundImage = new StyleBackground(iq.item.sprite);
            }
            else
            {
                // Draw empty cell
                Debug.Log($"DRAWING EMPTY CELL FOR ({row},{col})!");
                qtyLabel.text = "";
                rootButton.style.backgroundImage = StyleKeyword.None;
            }
        }
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
}

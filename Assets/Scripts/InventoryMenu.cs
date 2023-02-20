using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private string MenuTitle;
    [SerializeField] private const int numRows = 4, numCols = 6;

    // UI Elements
    private VisualElement root;
    private IMGUIContainer GridContainer;
    private Label title;
    private VisualElement[,] cellsByRow; // We assume these to be using InventoryCell.uxml

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");
        title = root.Q<Label>("Title");

        // Get rows and cells
        cellsByRow = new TemplateContainer[numRows,numCols];
        IMGUIContainer[] rows = new IMGUIContainer[numRows]
        {
            GridContainer.Query<IMGUIContainer>("Row1"),
            GridContainer.Query<IMGUIContainer>("Row2"),
            GridContainer.Query<IMGUIContainer>("Row3"),
            GridContainer.Query<IMGUIContainer>("Row4")
        };

        // Compose the 2D array of cells mapped to rows
        for(int r = 0; r < numRows; r++)
        {
            VisualElement[] children = rows[r].Children().ToArray();
            for (int c = 0; c < numCols; c++)
            {
                cellsByRow[r, c] = children[c];
            }
        }

        //DebugShowCellCoordinates();

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
            int col = i % numCols;
            int row = i / numCols;

            VisualElement cell = cellsByRow[row, col];
            Label qtyLabel = cell.Q<Label>("QuantityLabel");
            Button rootButton = cell.Q<Button>("RootButton");
            if (i < inventory.Stacks.Count)
            {
                // Draw in the item info
                ItemQuantity iq = inventory.Stacks[i];
                qtyLabel.text = iq.quantity.ToString();
                rootButton.style.backgroundImage = new StyleBackground(iq.item.sprite);
            }
            else
            {
                // Draw empty cell
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

    #region Debugging

    /**
     * For testing only. Set the label in each inventory cell to be 
     * its coordinates in the grid.
     */
    private void DebugShowCellCoordinates()
    {
        for (int r = 0; r < cellsByRow.Length; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                Label qtyLabel = cellsByRow[r, c].Q<Label>("QuantityLabel");
                qtyLabel.text = $"({r},{c})";
            }
        }
    }

    #endregion
}

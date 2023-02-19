using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryMenu : MonoBehaviour
{
    public VisualTreeAsset CellTemplate;
    public static PlayerInventoryMenu Instance { get; private set; }
    // UI Elements
    VisualElement root;
    IMGUIContainer GridContainer;
    private TemplateContainer[] cells;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;

        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");
        root.style.display = DisplayStyle.None;
        root.SetEnabled(false);
    }

    private void OnEnable()
    {
        //DrawInventory();
    }

    public void DrawInventory(Inventory inventory)
    {
        int stackSize = inventory.StackCapacity;
        cells = new TemplateContainer[stackSize];

        // Create empty cells
        for (int i = 0; i < stackSize; i++)
        {
            cells[i] = DrawEmptyCell();
        }

        // Replace empty cells with items
        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            GridContainer.Remove(cells[i]);
            cells[i] = DrawCellForItem(i, inventory.Stacks[i]);
        }
    }

    private TemplateContainer DrawCellForItem(int index, ItemQuantity item)
    {
        var cell = CellTemplate.Instantiate();
        Label quantityLabel = cell.Q<Label>("QuantityLabel");
        Button rootButton = cell.Q<Button>("RootButton");
        quantityLabel.text = item.quantity.ToString();
        rootButton.style.backgroundImage = new StyleBackground(item.item.sprite);
        GridContainer.Insert(index, cell);
        return cell;
    }

    private TemplateContainer DrawEmptyCell()
    {
        var cell = CellTemplate.Instantiate();
        Label quantityLabel = cell.Q<Label>("QuantityLabel");
        Button rootButton = cell.Q<Button>("RootButton");
        quantityLabel.text = "";
        rootButton.style.backgroundImage = new StyleBackground(StyleKeyword.None);
        GridContainer.Add(cell);
        return cell;
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
        else
        {
            GridContainer.Clear();
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryMenu : MonoBehaviour
{
    public VisualTreeAsset CellTemplate;
    public static PlayerInventoryMenu Instance;
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
        DontDestroyOnLoad(gameObject);

        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");
        root.style.display = DisplayStyle.None;
        root.SetEnabled(false);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        DrawInventory();
    }

    private void DrawInventory()
    {
        // Test example
        //for (int i = 0; i < 20; i++)
        //{
        //    var testCell = CellTemplate.Instantiate();
        //    Label quantityLabel = testCell.Q<Label>("QuantityLabel");
        //    Button rootButton = testCell.Q<Button>("RootButton");

        //    quantityLabel.text = itemQuantity.quantity.ToString();
        //    rootButton.style.backgroundImage = new StyleBackground(itemQuantity.item.sprite);

        //    GridContainer.Add(testCell);
        //}

        // @TODO pick up here. Draw the inventory cells!
        int stackSize = InventoryManager.Instance.PlayerInventory.StackCapacity;
        cells = new TemplateContainer[stackSize];

        // Create empty cells
        for (int i = 0; i < stackSize; i++)
        {
            cells[i] = DrawEmptyCell();
        }

        // Replace empty cells with items
        for (int i = 0; i < InventoryManager.Instance.PlayerInventory.Stacks.Count; i++)
        {
            GridContainer.Remove(cells[i]);
            cells[i] = DrawCellForItem(InventoryManager.Instance.PlayerInventory.Stacks[i]);
        }
    }

    private TemplateContainer DrawCellForItem(ItemQuantity item)
    {
        var cell = CellTemplate.Instantiate();
        Label quantityLabel = cell.Q<Label>("QuantityLabel");
        Button rootButton = cell.Q<Button>("RootButton");
        quantityLabel.text = item.quantity.ToString();
        rootButton.style.backgroundImage = new StyleBackground(item.item.sprite);
        GridContainer.Add(cell);
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
    public void ToggleMenuOpen()
    {
        bool newEnabledValue = !root.enabledInHierarchy;
        root.SetEnabled(newEnabledValue);
        StyleEnum<DisplayStyle> oldDisplayStyle = root.style.display;
        root.style.display = oldDisplayStyle == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;

        if (newEnabledValue)
        {
            Debug.Log("Toggled ON. Drawing Inventory...");
            DrawInventory();
        }
        else
        {
            Debug.Log("Toggled OFF.");
            GridContainer.Clear();
        }
    }

    
}

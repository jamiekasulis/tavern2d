using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryMenu : MonoBehaviour
{
    public VisualTreeAsset CellTemplate;
    public static PlayerInventoryMenu Instance;

    public ItemQuantity itemQuantity; // For testing only

    // UI Elements
    VisualElement root;
    IMGUIContainer GridContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");

        // Set up a single test item cell
        for (int i = 0; i < 20; i++)
        {
            var testCell = CellTemplate.Instantiate();
            Label quantityLabel = testCell.Q<Label>("QuantityLabel");
            Button rootButton = testCell.Q<Button>("RootButton");

            quantityLabel.text = itemQuantity.quantity.ToString();
            rootButton.style.backgroundImage = new StyleBackground(itemQuantity.item.sprite);

            GridContainer.Add(testCell);
        }
    }

    /**
     * Toggles both the enabled status and visibility of the menu.
     * Enabled: This is important because it will stop listening for most events
     * when disabled.
     * Visibility: Obviously we want to control when it's in the way :P
     */
    public void ToggleMenuOpen()
    {
        ToggleEnabled();
        ToggleVisibility();
    }

    private void ToggleEnabled()
    {
        bool oldValue = root.enabledInHierarchy;
        root.SetEnabled(!oldValue);
    }

    private void ToggleVisibility()
    {
        StyleEnum<DisplayStyle> oldValue = root.style.display;
        root.style.display = oldValue == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }
}

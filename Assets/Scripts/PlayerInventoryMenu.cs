using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryMenu : MonoBehaviour
{
    public VisualTreeAsset CellTemplate;
    public static PlayerInventoryMenu Instance;
    public string menuTitle = "Backpack";

    public ItemQuantity itemQuantity; // For testing only

    // UI Elements
    VisualElement root;
    IMGUIContainer GridContainer, TopbarContainer;
    Label TitleLabel;

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
        TopbarContainer = root.Q<IMGUIContainer>("TopbarContainer");
        TitleLabel = TopbarContainer.Q<Label>("Title");

        TitleLabel.text = menuTitle;

        // Set up a single test item cell
        var testCell = CellTemplate.Instantiate();
        Label QuantityLabel = testCell.Q<Label>("QuantityLabel");
        VisualElement ItemSprite = testCell.Q<VisualElement>("ItemSprite");

        QuantityLabel.text = itemQuantity.quantity.ToString();
        Image img = new();
        img.sprite = itemQuantity.item.sprite;
        ItemSprite.Add(img);

        GridContainer.Add(testCell);
    }

    /**
     * Toggles both the enabled status and visibility of the menu.
     * Enabled: This is important because it will stop listening for most events
     * when disabled.
     * Visibility: Obviously we want to control when it's in the way :P
     */
    public void ToggleMenuOpen()
    {
        // Toggle Enabled
        bool oldValue = root.enabledInHierarchy;
        root.SetEnabled(!oldValue);
        // Toggle visibility
        root.style.display = oldValue ? DisplayStyle.None : DisplayStyle.Flex;
    }
}

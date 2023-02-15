using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventoryMenu : MonoBehaviour
{
    public VisualTreeAsset CellTemplate;
    public static PlayerInventoryMenu instance;
    public ItemQuantity itemQuantity; // For testing only

    // UI Elements
    VisualElement root;
    IMGUIContainer GridContainer, TopbarContainer;
    Label TitleLabel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        GridContainer = root.Q<IMGUIContainer>("GridContainer");
        TopbarContainer = root.Q<IMGUIContainer>("TopbarContainer");
        TitleLabel = TopbarContainer.Q<Label>("Title");

        TitleLabel.text = "Backpack";

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
}

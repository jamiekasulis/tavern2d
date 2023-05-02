using UnityEngine;
using UnityEngine.UIElements;

/**
 * This is a UI component that will show the item currently picked up as a 
 * tooltip around the mouse.
 * This currently has a specific usage which is the rearrangement of inventory
 * using the inventory UI, but as the game develops there might be a good
 * reason to refactor this into a more general mouse tooltip.
 */
[RequireComponent(typeof(UIDocument))]
public class RearrangeInventoryTooltip : MonoBehaviour
{
    private bool IsShowingImage;
    private VisualElement rootButton, root;
    private Label quantityLabel;
    private Color transparentGray = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray with 50% transparency

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        rootButton = root.Q<Button>("RootButton");
        quantityLabel = rootButton.Q<Label>("QuantityLabel");
        rootButton.style.unityBackgroundImageTintColor = transparentGray;
        rootButton.style.backgroundColor = transparentGray;

        Clear();
    }

    public void Draw(ItemQuantity? iq)
    {
        if (iq == null)
        {
            Clear();
            return;
        }
        rootButton.style.backgroundImage = new StyleBackground(iq.item.spriteFront);
        quantityLabel.text = iq.quantity.ToString();
        IsShowingImage = true;
        rootButton.style.display = DisplayStyle.Flex;
    }

    public void Clear()
    {
        rootButton.style.display = DisplayStyle.None;
        quantityLabel.text = "";
        IsShowingImage = false;
    }

    private void Update()
    {
        if (IsShowingImage)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 mousePositionCorrected = new(mousePosition.x, Screen.height - mousePosition.y);
            mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(root.panel, mousePositionCorrected);
            root.transform.position = mousePositionCorrected;
        }
    }
}

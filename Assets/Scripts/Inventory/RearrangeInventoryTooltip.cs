using UnityEngine;
using UnityEngine.UIElements;

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

        Debug.Log($"Drawing inventory tooltip");
        rootButton.style.backgroundImage = new StyleBackground(iq.item.spriteFront);
        quantityLabel.text = iq.quantity.ToString();
        IsShowingImage = true;
        rootButton.style.display = DisplayStyle.Flex;
    }

    public void Clear()
    {
        Debug.Log($"Clearing inventory tooltip");
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

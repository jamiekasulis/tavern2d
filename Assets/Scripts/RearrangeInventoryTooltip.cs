using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class RearrangeInventoryTooltip : MonoBehaviour
{
    private bool IsShowingImage;
    private VisualElement rootButton, root;
    private Label quantityLabel;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        rootButton = root.Q<Button>("RootButton");
        quantityLabel = rootButton.Q<Label>("QuantityLabel");
        rootButton.style.unityBackgroundImageTintColor = Color.gray;

        Clear();
    }

    public void Draw(ItemQuantity iq)
    {
        rootButton.style.backgroundImage = new StyleBackground(iq.item.sprite);
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
            Vector2 mousePositionCorrected = new(mousePosition.x, Screen.height - mousePosition.y - 150);
            mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(root.panel, mousePositionCorrected);
            root.transform.position = mousePositionCorrected;
        }
    }
}

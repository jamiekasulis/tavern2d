using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class RearrangeInventoryTooltip : MonoBehaviour
{
    private bool IsShowingImage;
    private VisualElement mainElement, root;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        mainElement = root.Q<VisualElement>("RootElement");
        IsShowingImage = false;
        mainElement.style.unityBackgroundImageTintColor = Color.gray;
    }

    public void Draw(Sprite sprite)
    {
        mainElement.style.backgroundImage = new StyleBackground(sprite);
        IsShowingImage = true;
        mainElement.style.display = DisplayStyle.Flex;
    }

    public void Clear()
    {
        mainElement.style.display = DisplayStyle.None;
        IsShowingImage = false;
    }

    private void Update()
    {
        if (IsShowingImage)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 mousePositionCorrected = new Vector2(mousePosition.x, Screen.height - mousePosition.y - 150);
            mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(root.panel, mousePositionCorrected);
            root.transform.position = mousePositionCorrected;
        }
    }
}

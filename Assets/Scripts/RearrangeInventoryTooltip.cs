using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RearrangeInventoryTooltip : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;
    private bool IsShowingImage;

    private void Awake()
    {
        var backgroundTransform = transform.Find("Background");
        rectTransform = backgroundTransform.GetComponent<RectTransform>();
        image = backgroundTransform.GetComponent<Image>();
        image.enabled = false;
        IsShowingImage = false;
    }

    public void Draw(Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = true;
        IsShowingImage = true;
    }

    public void Clear()
    {
        image.sprite = null;
        IsShowingImage = false;
        image.enabled = false;
    }

    private void Update()
    {
        if (IsShowingImage)
        {
            rectTransform.position = Input.mousePosition + new Vector3(50, 50, 0); // Offset its position so it's not blocking the mouse from clicking other things
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * This class contains the build mode-related functionailities of the inventory
 * menu.
 */
public class BuildModeInventoryMenu : MonoBehaviour
{
    public void SetPlaceableObjectHighlight(Item? item, VisualElement visual)
    {
        bool shouldHighlight =
            BuildMode.Instance.IsEnabled &&
            item != null &&
            item.buildMode;

        if (shouldHighlight)
        {
            visual.style.borderTopWidth = new StyleFloat(2);
            visual.style.borderTopColor = InventoryMenu.HighlightColor;

            visual.style.borderBottomWidth = new StyleFloat(2);
            visual.style.borderBottomColor = InventoryMenu.HighlightColor;

            visual.style.borderLeftWidth = new StyleFloat(2);
            visual.style.borderLeftColor = InventoryMenu.HighlightColor;

            visual.style.borderRightWidth = new StyleFloat(2);
            visual.style.borderRightColor = InventoryMenu.HighlightColor;
        }
        else
        {
            visual.style.borderTopColor = Color.clear;
            visual.style.borderTopWidth = StyleKeyword.None;

            visual.style.borderBottomColor = Color.clear;
            visual.style.borderBottomWidth = StyleKeyword.None;

            visual.style.borderLeftColor = Color.clear;
            visual.style.borderLeftWidth = StyleKeyword.None;

            visual.style.borderRightColor = Color.clear;
            visual.style.borderRightWidth = StyleKeyword.None;
        }
    }

    public void OnCellClick()
    {
        // @TODO implement me!
    }
}

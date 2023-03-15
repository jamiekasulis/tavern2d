using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * This class contains the build mode-related functionailities of the inventory
 * menu.
 */
public class BuildModeInventoryMenuTriggers : MonoBehaviour
{
    /**
     * Determines if the given visual element should be highlighted
     * (if build mode is ON and the item is a build mode-approved item.)
     * Then highlights the item.
     * 
     * If it is determined the item should NOT be highlighted,
     * any highlights will be removed.
     */
    public void SetHighlightForInventoryCell(Item? item, VisualElement visualElt)
    {
        bool shouldHighlight =
            BuildMode.Instance.IsEnabled &&
            item != null &&
            item.buildMode;

        Button btn = visualElt.Q<Button>("RootButton");

        if (shouldHighlight)
        {
            btn.style.borderTopWidth = new StyleFloat(2);
            btn.style.borderTopColor = InventoryMenu.HighlightColor;

            btn.style.borderBottomWidth = new StyleFloat(2);
            btn.style.borderBottomColor = InventoryMenu.HighlightColor;

            btn.style.borderLeftWidth = new StyleFloat(2);
            btn.style.borderLeftColor = InventoryMenu.HighlightColor;

            btn.style.borderRightWidth = new StyleFloat(2);
            btn.style.borderRightColor = InventoryMenu.HighlightColor;
        }
        else
        {
            btn.style.borderTopColor = Color.clear;
            btn.style.borderTopWidth = StyleKeyword.None;

            btn.style.borderBottomColor = Color.clear;
            btn.style.borderBottomWidth = StyleKeyword.None;

            btn.style.borderLeftColor = Color.clear;
            btn.style.borderLeftWidth = StyleKeyword.None;

            btn.style.borderRightColor = Color.clear;
            btn.style.borderRightWidth = StyleKeyword.None;
        }
    }

    /**
     * See <see cref="SetHighlightForInventoryCell"/>
     */
    public void SetHighlightForAllInventoryCells(List<(Item?, VisualElement)> itemsToVisualElt) {
        itemsToVisualElt.ForEach(i =>
            SetHighlightForInventoryCell(i.Item1, i.Item2)
        );
    }

    public void OnCellClick()
    {
        // @TODO implement me!
    }
}

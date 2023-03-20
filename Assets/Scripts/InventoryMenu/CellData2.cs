using System.Collections.Generic;
using UnityEngine.UIElements;

public class CellData2
{
    public VisualElement visualElement;
    public ItemQuantity? itemData;
    public int row, col;
    public bool buttonEnabled;
    public List<InventoryCellStyleEnum> additionalStyles;

    public enum InventoryCellStyleEnum
    {
        BuildModeOK = 0,
        BuildModeNotOK = 1
    }

    public CellData2(VisualElement cellVisualElement, ItemQuantity? itemData, int row, int column)
    {
        visualElement = cellVisualElement;
        this.itemData = itemData;
        this.row = row;
        col = column;
        buttonEnabled = true;
        additionalStyles = new List<InventoryCellStyleEnum>();
    }

    public CellData2(VisualElement cellVisualElement, ItemQuantity? itemData, int row, int column, List<InventoryCellStyleEnum> additionalStyles)
    {
        visualElement = cellVisualElement;
        this.itemData = itemData;
        this.row = row;
        col = column;
        buttonEnabled = true;
        this.additionalStyles = additionalStyles;
    }
}
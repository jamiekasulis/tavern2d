using UnityEngine;

[System.Serializable]
public class ItemQuantity
{
    public Item item;
    [Range(1, 999)]
    public int quantity;

    public override string ToString()
    {
        return $"({item.itemName},{quantity})";
    }
}

using UnityEngine;

public class ItemQuantity
{
    [SerializeField] public Item item;
    [Range(1, 999)]
    [SerializeField] public int quantity;
}

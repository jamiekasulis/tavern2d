using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public Inventory Inventory { get; private set; }
    [SerializeField] public string playerName = "Player1";
    [SerializeField] public List<ItemQuantity> testInventoryContents;

    private void Awake()
    {
        Inventory = new Inventory(10);
        foreach (var iq in testInventoryContents)
        {
            Inventory.Add(iq);
        }
    }
}

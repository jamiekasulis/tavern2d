using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] public Inventory Inventory { get; private set; }
    [SerializeField] public string playerName = "Player1";
    [SerializeField] public List<ItemQuantity> testInventoryContents;
    [SerializeField] public float speed = 0.07f;

    public Rigidbody2D RigidBody;

    private void Awake()
    {
        RigidBody = gameObject.GetComponent<Rigidbody2D>();

        Inventory = new Inventory(10);
        foreach (var iq in testInventoryContents)
        {
            Inventory.Add(iq);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PickUp pickup;
    public Vector2 facedDirection { get; private set; }
    private List<Vector2> validDirections = new(8)
    {
        // cardinals
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        // diagonals
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    };

    // Item pickup
    public Vector2 boxcastSize = new(1, 1);
    public float boxcastDistance = 1f;

    private void Awake()
    {
        facedDirection = Vector2.down;
    }

    private void Update()
    {
        DetectPickUp();
        HandleManualPickup();
    }

    private void DetectPickUp()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(gameObject.transform.position, boxcastSize, 0, facedDirection, boxcastDistance);

        // Since we get hits as an array, the most space efficient way to do what
        // we want is to iterate through the array looking for our hit and return
        // once we find the first one. The alternative is to convert the arr to
        // a list and filter, which is more readable but more memory to allocate.
        for (int i = 0; i < hits.Length; i++)
        {
            PickUp maybePickUp = hits[i].collider.gameObject.GetComponent<PickUp>();
            if (maybePickUp != null)
            {
                pickup = maybePickUp;
                Debug.Log("Detected pickup " + pickup.gameObject.name);
                return;
            }
        }
        pickup = null;
        return;
    }

    private void HandleManualPickup()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.PICKUP_ITEM))
        {
            if (pickup != null)
            {
                pickup.AddToPlayerInventory();
                Debug.Log("Player Inventory: " + InventoryManager.instance.PlayerInventory.ToString());
            }
        }
    }

    public void SetFacedDirection(Vector2 direction)
    {
        if (validDirections.Contains(direction))
        {
            // @TODO Also swap the sprite
            facedDirection = direction;
        }
        else
        {
            throw new InvalidDirectionException("Given direction " + direction.ToString() + " is not an accepted direction to face");
        }
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        DrawBoxcast();
    }

    private void DrawBoxcast()
    {
        // @TODO at cafe
    }

    #endregion
}

public class InvalidDirectionException : Exception
{
    public InvalidDirectionException(string message) : base(message) { }
}

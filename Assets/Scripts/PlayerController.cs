using System;
using System.Collections;
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
    private Vector2 boxcastSize = new(3, 3);

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
        Physics2D.BoxCastAll(gameObject.transform.position, boxcastSize, 0, facedDirection);
    }

    private void HandleManualPickup()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.PICKUP_ITEM))
        {

        }
    }

    public void SetFacedDirection(Vector2 direction)
    {
        if (validDirections.Contains(direction))
        {
            // @TODO Also swap the sprite
            facedDirection = direction;
            Debug.Log("Set faced direction to " + direction.ToString());
        }
        else
        {
            throw new InvalidDirectionException("Given direction " + direction.ToString() + " is not an accepted direction to face");
        }
    }
}

public class InvalidDirectionException : Exception
{
    public InvalidDirectionException(string message) : base(message) { }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    // UnityEvent stuff
    [SerializeField] private UnityEvent<PickUp> pickupEvent;

    private void Awake()
    {
        facedDirection = Vector2.down;
    }

    private void Update()
    {
        DetectPickUpAndPickUpAutomatic();
        HandleManualPickup();
        HandleToggleInventoryMenu();
    }

    /**
     * Picks up all automatic pickup items in the boxcast.
     * Sets the pickup variable to be the first hit in the boxcast.
     * Sets the pickup variable to null if there are no hits.
     */
    private void DetectPickUpAndPickUpAutomatic()
    {

        RaycastHit2D[] hits = Physics2D.BoxCastAll(gameObject.transform.position, boxcastSize, 0, facedDirection, boxcastDistance);
        if (hits.Length == 0)
        {
            pickup = null;
            return;
        }

        /*
         * We want to set the manual pick up item to be the FIRST non-automatic boxcast hit.
         * There may be more readable ways to do this by getting a list of hits that are pickups,
         * then filtering that by automatic-- but this seems like the most space- and time-efficient way
         * to do it.
         */
        bool setManualPickup = false;
        foreach (var hit in hits)
        {
            bool hasPickup = hit.collider.gameObject.TryGetComponent(out PickUp pu);
            if (hasPickup)
            {
                if (pu.automaticPickup)
                {
                    pu.AddToPlayerInventory();
                }
                else
                {
                    if (!setManualPickup)
                    {
                        pickup = pu;
                        setManualPickup = true;
                    }
                }
            }
        }

    }

    private void HandleManualPickup()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.PICKUP_ITEM))
        {
            if (pickup != null)
            {
                pickupEvent.Invoke(pickup);
                pickup = null;
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

    private void HandleToggleInventoryMenu()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_INVENTORY_MENU))
        {
            InventoryManager.Instance.TogglePlayerInventoryMenuEnabled();
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

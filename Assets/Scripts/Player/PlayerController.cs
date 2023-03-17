using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private PickUp pickup;
    private Interactable interactable;

    public Vector2 facedDirection { get; private set; }
    private List<Vector2> validDirections = new(8)
    {
        // cardinals
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        // diagonals
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    };

    // Item pickup
    public Vector2 pickupBoxCastSize = new(1, 1);
    public float pickupBoxCastDistance = 1f;

    // Interaction
    public Vector2 interactBoxCastSize = new(0.5f, 0.5f);
    public float interactBoxCastDistance = 0.5f;

    // UnityEvent stuff
    [SerializeField] private UnityEvent<PickUp> pickupEventTrigger;

    private void Awake()
    {
        facedDirection = Vector2.down;
    }

    private void Update()
    {
        DetectPickUpAndPickUpAutomatic();
        HandleManualPickup();
        HandleToggleInventoryMenu();

        DetectInteraction();
        HandleInteraction();
    }

    /**
     * Picks up all automatic pickup items in the boxcast.
     * Sets the pickup variable to be the first hit in the boxcast.
     * Sets the pickup variable to null if there are no hits.
     */
    private void DetectPickUpAndPickUpAutomatic()
    {

        RaycastHit2D[] hits = Physics2D.BoxCastAll(gameObject.transform.position, pickupBoxCastSize, 0, facedDirection, pickupBoxCastDistance);
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
                    pickupEventTrigger.Invoke(pu);
                    pickup = null;
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

    private void DetectInteraction()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(gameObject.transform.position, interactBoxCastSize, 0, facedDirection, interactBoxCastDistance);

        // No hits
        if (hits.Length == 0)
        {
            interactable = null;
            return;
        }

        // Check hits and let the select the one that is IInteractable
        foreach (var hit in hits)
        {
            bool isInteractable = hit.collider.gameObject.TryGetComponent<Interactable>(out Interactable i);
            if (isInteractable)
            {
                interactable = i;
                return;
            }
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.INTERACT) && interactable != null)
        {
            interactable.Interact();
        }
    }

    private void HandleManualPickup()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.PICKUP_ITEM))
        {
            if (pickup != null)
            {
                pickupEventTrigger.Invoke(pickup);
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

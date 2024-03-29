using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public PickUp pickup;
    public Interactable interactable { get; private set; }

    public Vector2 facedDirection { get; private set; }

    // @TODO Refactor direction-facing logic and variables, as these will be
    // used for NPCs as well.
    private List<Vector2> VALID_DIRECTIONS = new(8)
    {
        // cardinals
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        // diagonals
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    };

    // Item pickup
    public Vector2 pickupBoxCastSize = new(1, 1);
    public float pickupBoxCastDistance = 1f;
    [SerializeField] private UnityEvent<PickUp> PickedUp;

    // Interaction
    public Vector2 interactBoxCastSize = new(0.5f, 0.5f);
    public float interactBoxCastDistance = 0.5f;

    private void Awake()
    {
        facedDirection = Vector2.down;
    }

    private void Update()
    {
        DetectPickUpAndPickUpAutomatic();
        DetectInteraction();
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
                    PickedUp.Invoke(pu);
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

    public void SetFacedDirection(Vector2 direction)
    {
        Vector2 dirNormalized = new (
            direction.x < 0 ? -1
                : direction.x == 0 ? 0
                : 1,
            direction.y < 0 ? -1
                : direction.y == 0 ? 0
                : 1
        );

        if (VALID_DIRECTIONS.Contains(dirNormalized))
        {
            // @TODO Also swap the sprite
            facedDirection = dirNormalized;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player collided with " + collision.gameObject.name);
        HandleCollisionWithPickUp(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollisionWithPickUp(collision);
    }

    private void HandleCollisionWithPickUp(Collider2D collision)
    {
        PickUp pickupComponent = collision.gameObject.GetComponent<PickUp>();
        if (pickupComponent)
        {
            if (pickupComponent.automaticPickup)
            {
                pickupComponent.AddToPlayerInventory();

                // Debug logs
                Debug.Log("Automatically picked up " + pickupComponent.itemQuantity.ToString());
            }
            else
            {
                if (Input.GetKeyDown(MouseKeyboardControlsMapping.PICKUP_ITEM))
                {
                    pickupComponent.AddToPlayerInventory();
                    Debug.Log("Manually picked up " + pickupComponent.itemQuantity.ToString());
                }
            }
        }
    }
}

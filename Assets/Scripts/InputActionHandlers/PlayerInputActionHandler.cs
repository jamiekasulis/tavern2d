using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/**
 * This is a layer of abstraction that works with Unity's new 
 * UnityEngine.InputSystem to convert controller inputs (gamepad, keyboard,
 * mouse, etc) into in-game actions. These connections are wired in the Unity
 * Editor.
 */
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerInputActionHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent<PickUp> PickedUp;

    private PlayerController playerController;
    private Player player;
    private Vector2 movementDirection = Vector2.zero;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        player = gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /*
     * Movement happens continuously via Move(). This function resets the direction of movement.
     */
    public void OnMove(InputValue value)
    {
        movementDirection = value.Get<Vector2>();
    }

    private void Move()
    {
        playerController.SetFacedDirection(new(movementDirection.x, movementDirection.y));

        float playerSpeed = player.speed;
        Vector2 difference = new Vector3(playerSpeed * movementDirection.x, playerSpeed * movementDirection.y);
        if (difference.magnitude > playerSpeed)
        {
            difference = Vector2.ClampMagnitude(difference, playerSpeed);
        }
        player.RigidBody.MovePosition(player.RigidBody.position + difference);
    }

    public void OnInteract()
    {
        playerController.interactable?.Interact();
    }

    public void OnPickUpItem()
    {
        if (playerController.pickup != null)
        {
            PickedUp.Invoke(playerController.pickup);
            playerController.pickup = null;
        }
    }

    public void OnToggleBuildMode()
    {
        if (BuildMode.Instance.IsEnabled)
        {
            GameEvents.Instance.OnBuildModeEnabled();
        }
        else
        {
            GameEvents.Instance.OnBuildModeDisabled();
        }
        
    }

    public void OnRotateLeft()
    {
        if (BuildMode.Instance.IsEnabled)
        {
            BuildMode.Instance.RotateObject(PlaceableObject.RotationDirectionEnum.Left);
        }
    }

    public void OnRotateRight()
    {
        if (BuildMode.Instance.IsEnabled)
        {
            BuildMode.Instance.RotateObject(PlaceableObject.RotationDirectionEnum.Right);
        }
    }

    public void OnCancelGeneral()
    {
        if (BuildMode.Instance.IsPlacingObject())
        {
            BuildMode.Instance.CancelPlacement();
        }
}

}

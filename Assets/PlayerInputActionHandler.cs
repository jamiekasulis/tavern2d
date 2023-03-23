using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Handles InputAction behavior.
 */
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerInputActionHandler : MonoBehaviour
{
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
        Debug.Log("InputValue is " + value.Get<Vector2>().ToString());
        movementDirection = value.Get<Vector2>();
    }

    private void Move()
    {
        //playerController.SetFacedDirection(new(movementDirection.x, movementDirection.y));

        float playerSpeed = player.speed;
        Vector2 difference = new Vector3(playerSpeed * movementDirection.x, playerSpeed * movementDirection.y);
        if (difference.magnitude > playerSpeed)
        {
            difference = Vector2.ClampMagnitude(difference, playerSpeed);
        }
        player.RigidBody.MovePosition(player.RigidBody.position + difference);
    }


}

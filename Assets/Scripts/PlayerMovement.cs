using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.07f;
    private float xDirection = 0, yDirection = 0;
    private Rigidbody2D rigidBody;
    private PlayerController playerController;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        playerController = gameObject.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        xDirection = Input.GetAxisRaw("Horizontal");
        yDirection = Input.GetAxisRaw("Vertical");

        if (xDirection == 0 && yDirection == 0)
        {
            return;
        }

        playerController.SetFacedDirection(new(xDirection, yDirection));

        Vector2 difference = new (speed * xDirection, speed * yDirection);

        /**
         * Moving diagonally can cause you to go faster. This is due to
         * trigonometry. To avoid this, clamp the magnitude of the vector
         * to the speed.
         */
        if (difference.magnitude > speed)
        {
            difference = Vector2.ClampMagnitude(difference, speed);
        }
        
        rigidBody.MovePosition(rigidBody.position + difference);
        
    }
}

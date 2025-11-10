using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 3; // Tiles per second
    [SerializeField] float framesToMaxSpeed = 5;
    [SerializeField] float framesToStill = 3;

    Rigidbody2D rb;

    FacingDirection facing = FacingDirection.right;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

    }

    public bool IsWalking()
    {
        return rb.linearVelocityX != 0 && IsGrounded();
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        if (rb.linearVelocityX == 0)                // Return current facing direction if not moving
        {
            return facing;
        } else if (rb.linearVelocityX > 0)          // If player is moving right, look right
        {
            return facing = FacingDirection.right;
        } else                                      // Otherwise player is facing left, so look left
        {
            return facing = FacingDirection.left;
        }
    }
}

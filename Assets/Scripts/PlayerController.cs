using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] float apexHeight = 3;
    [SerializeField] float apexTime = 1;
    [SerializeField] float fallTime = 0.75f;
    [SerializeField] float terminalVelocity = -8;
    private float ascendGravity;
    private float fallGravity;

    private float jumpProgress = 0;

    [SerializeField] float coyoteTime = 1.25f;
    private float coyoteProgress = 0;

    [Header("Horizontal Movement")]
    [SerializeField] float maxHorizontalSpeed = 3; // Tiles per second

    [SerializeField] int framesToSpeed = 5;
    [SerializeField] float accelModifier = 0.4f;

    [SerializeField] int framesToStop = 3;
    [SerializeField] float decelModifier = 1.8f;

    [SerializeField] int framesToTurn = 2;
    [SerializeField] float turnModifier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] int raycastCount = 12;
    [SerializeField] float raycastDist = 0.05f;
    [SerializeField] float raycastMargins = 0.01f;

    Rigidbody2D rb;

    BoxCollider2D collider;

    FacingDirection facing = FacingDirection.right; // Default facing direction

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        ascendGravity = -2 * apexHeight / (apexTime * apexTime);
        fallGravity = -2 * apexHeight / (fallTime * fallTime);
    }

    void FixedUpdate()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        // Horizontal Movement
        if (playerInput.x != 0)
            Accelerate(playerInput);
        else
            Decelerate();

        // Vertical Movement
        if (playerInput.y == 1)
            Jump();
        else
            NotJumping();

        Fall();
        
    }

    private void Accelerate(Vector2 playerInput)
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(playerInput.x);

        // Find current time-position of x velocity
        float timePosition = CurrentAccelTime(rb.linearVelocityX * sign);
        
        // Find next position of x velocity
        float newSpeed = CalculateAccelSpeed(timePosition + playerInput.x * sign);

        // Set rigidbody's velocity
        rb.linearVelocityX = newSpeed * sign;
    }

    private void Decelerate()
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(rb.linearVelocityX);

        // Find current time-position of x velocity
        float timePosition = CurrentDecelTime(rb.linearVelocityX * sign);
        
        // Find next position of x velocity
        float newSpeed = CalculateDecelSpeed(timePosition + 1);

        // Set rigidbody's velocity
        rb.linearVelocityX = newSpeed * sign;
    }

    private void Jump()
    {
        if (IsGrounded() || coyoteProgress < coyoteTime)
        {
            // Set initial vertical velocity
            rb.linearVelocityY = 2 * apexHeight / apexTime;
            // Reset jump progress
            jumpProgress = 0;
            // Ensure coyote time doesn't retrigger
            coyoteProgress += coyoteTime;
        }
    }

    private void NotJumping()
    {
        // Player isn't jumping, so set jumpProgress to "completed" time-value
        jumpProgress = apexTime;

        // Increase coyote time progress
        coyoteProgress += Time.fixedDeltaTime;
        // If player is on the ground, reset coyote time
        if (IsGrounded())
        {
            coyoteProgress = 0;
        }
    }

    private void Fall()
    {
        // If player is still jumping, use ascension gravity
        if (jumpProgress < apexTime)
            rb.linearVelocityY += ascendGravity * Time.fixedDeltaTime;
        // Otherwise use fast-falling gravity
        else
            rb.linearVelocityY += fallGravity * Time.fixedDeltaTime;

        // Increase jump time
        jumpProgress += Time.fixedDeltaTime;

        // Limit falling speed to terminalVelocity
        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, terminalVelocity);
    }

    public bool IsWalking()
    {
        return Mathf.Abs(rb.linearVelocityX) > 0.05f && IsGrounded();
    }

    public bool IsGrounded()
    {
        // Get position of first raycast (bottom-left of collider) and offset for each raycast
        Vector2 startPos = (Vector2) transform.position + collider.offset + Vector2.one * raycastMargins - collider.size / 2;
        Vector2 offset = Vector2.right * (collider.size.x - 2 * raycastMargins) / (raycastCount - 1);

        // Check if any of the hits were ground
        //      If so return true
        //      Otherwise, return false
        for (int i = 0; i < raycastCount; i++)
        {
            Vector2 start = startPos + i * offset;
            RaycastHit2D hit = Physics2D.Linecast(start, start + Vector2.down * raycastDist, LayerMask.GetMask("Ground"));
            if (hit)
                return true;
        }
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        if (Mathf.Abs(rb.linearVelocityX) < 0.05f)  // Return current facing direction if not moving
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

    private float CurrentAccelTime(float speed)
    {
        return 
            speed < maxHorizontalSpeed ?
                speed >= 0 ?
                // Normal acceleration curve
                InverseSimplePolynomial(speed, maxHorizontalSpeed, framesToSpeed, accelModifier) :
                // Turning acceleration curve
                -InverseSimplePolynomial(-speed, maxHorizontalSpeed, Mathf.Min(framesToTurn, framesToStop), turnModifier) :
            framesToSpeed;
    }

    private float CurrentDecelTime(float speed)
    {
        // Deceleration curve
        return 
            Mathf.Abs(speed) > 0.05f ?
            -InverseSimplePolynomial(speed, maxHorizontalSpeed, framesToStop, decelModifier) + framesToStop:
            framesToStop;
    }

    private float CalculateAccelSpeed(float time)
    {
        return 
            time <= framesToSpeed ?
                time >= 0 ?
                SimplePolynomial(time, maxHorizontalSpeed, framesToSpeed, accelModifier) :
                SimplePolynomial(-time, -maxHorizontalSpeed, Mathf.Min(framesToTurn, framesToStop), turnModifier) :
            maxHorizontalSpeed;
    }

    private float CalculateDecelSpeed(float time)
    {
        return 
            time <= framesToStop ?
            SimplePolynomial(-time + framesToStop, maxHorizontalSpeed, framesToStop, decelModifier) :
            0;
    }

    private float SimplePolynomial(float value, float coef, float length, float exp)
    {
        return coef * Mathf.Pow(value / length, exp);
    }

    private float InverseSimplePolynomial(float value, float coef, float length, float exp)
    {
        return length * Mathf.Pow(value / coef, 1 / exp);
    }
}

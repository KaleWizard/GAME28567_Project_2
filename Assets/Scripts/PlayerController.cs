using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float maxHorizontalSpeed = 3; // Tiles per second

    [SerializeField] int framesToSpeed = 5;
    [SerializeField] float accelModifier = 0.4f;

    [SerializeField] int framesToStop = 3;
    [SerializeField] float decelModifier = 1.8f;

    [SerializeField] int framesToTurn = 2;
    [SerializeField] float turnModifier = 0.5f;

    Rigidbody2D rb;

    FacingDirection facing = FacingDirection.right; // Default facing direction

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
        if (playerInput.x != 0)
        {
            Accelerate(playerInput);
        } else
        {
            Decelerate();
        }
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

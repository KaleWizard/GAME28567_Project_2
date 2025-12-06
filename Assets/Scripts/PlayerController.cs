using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("General Properties")]
    internal Rigidbody2D rb;

    new internal BoxCollider2D collider;

    internal FacingDirection facing = FacingDirection.right; // Default facing direction

    private PlayerInput input = new();

    public SpriteRenderer spriteRenderer;

    [Header("Vertical Movement")]
    public float apexHeight = 3;
    public float apexTime = 1;
    public float fallTime = 0.75f;
    public float terminalVelocity = -8;
    internal float ascendGravity;
    internal float fallGravity;

    internal float jumpProgress = 0;

    public float coyoteTime = 1.25f;
    internal float coyoteProgress = 0;

    [Header("Horizontal Movement")]
    public float maxHorizontalSpeed = 3; // Tiles per second

    public int framesToSpeed = 5;
    public float accelModifier = 0.4f;

    public int framesToStop = 3;
    public float decelModifier = 1.8f;

    public int framesToTurn = 2;
    public float turnModifier = 0.5f;

    [Header("Dash")]
    public float dashTotalDist = 3;

    public int dashFramesToSpeed = 3;
    public float dashAccelModifier = 0.5f;

    public int dashFramesTotal = 9;
    public int dashFramesToControl = 3;

    public BoxCollider2D terrainCheckCollider;
    public float dashDistAfterTerrain = 0.25f;

    [Header("Ladder Movement")]
    public float ladderMaxSpeedX = 0.8f;
    public float ladderMaxSpeedY = 2.5f;

    public int ladderFramesToSpeed = 3;
    public float ladderAccelModifier = 0.5f;

    public int ladderFramesToStop = 3;
    public float ladderDecelModifier = 0.5f;

    internal bool exitLadderJump = false;

    [Header("Bouncy Ball Transformation")]
    public CircleCollider2D ballCollider;
    public float ballTerminalVelocity = -16;
    public SpriteRenderer ballSpriteRenderer;

    [Header("Ground Check")]
    public int raycastCount = 12;
    public float raycastDist = 0.05f;
    public float raycastMargins = 0.01f;

    public enum FacingDirection
    {
        left, right
    }

    // Player's current state
    internal BaseState state;

    // Player states
    internal BasicMovementState BasicMovementState = new();
    internal DashingState DashingState = new();
    internal ClimbingState ClimbingState = new();
    internal BouncyBallState BouncyBallState = new();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        ascendGravity = -2 * apexHeight / (apexTime * apexTime);
        fallGravity = -2 * apexHeight / (fallTime * fallTime);

        BasicMovementState.Initialize(this);
        DashingState.Initialize(this);
        ClimbingState.Initialize(this);
        BouncyBallState.Initialize(this);

        state = BasicMovementState;
        state.EnterState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            input.jumpInput = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) input.dashInput = true;
        if (Input.GetKeyDown(KeyCode.B)) input.toBallInput = true;
    }

    void FixedUpdate()
    {
        input.direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space))
            input.direction.y = 1;
        state.Update(input);
        ResetInput();
    }

    public void SwapState(BaseState newState)
    {
        state.ExitState();
        state = newState;
        state.EnterState();
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

    public bool IsInLadder()
    {
        foreach (BoxCollider2D ladder in LadderRegister.Instance.ladderColliders)
        {
            if (ladder.bounds.Contains(transform.position)) return true;
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

    public float SimplePolynomial(float value, float coef, float length, float exp)
    {
        return coef * Mathf.Pow(value / length, exp);
    }

    public float InverseSimplePolynomial(float value, float coef, float length, float exp)
    {
        return length * Mathf.Pow(value / coef, 1 / exp);
    }

    private void ResetInput()
    {
        input = new();
    }
}

public struct PlayerInput
{
    public Vector2 direction;
    public bool jumpInput;
    public bool dashInput;
    public bool toBallInput;
}

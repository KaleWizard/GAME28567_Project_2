using UnityEngine;

public class BasicMovementState : BaseState
{
    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
    }
    public override void EnterState()
    {
        if (parent.exitLadderJump) Jump();
        parent.exitLadderJump = false;
    }
    public override void Update(PlayerInput playerInput)
    {
        // Horizontal Movement
        if (playerInput.direction.x != 0)
            Accelerate(playerInput.direction);
        else
            Decelerate();

        // Vertical Movement
        if (playerInput.jumpInput)
            Jump();

        if (playerInput.direction.y != 1)
            NotJumping();

        Fall();

        if (playerInput.dashInput)
            parent.SwapState(parent.DashingState);

        if (parent.IsInLadder() && playerInput.direction.y != 0 && parent.rb.linearVelocityY <= 0) 
            parent.SwapState(parent.ClimbingState);
    }
    public override void ExitState()
    {

    }

    private void Accelerate(Vector2 playerInput)
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(playerInput.x);

        // Find current time-position of x velocity
        float timePosition = CurrentAccelTime(parent.rb.linearVelocityX * sign);

        // Find next position of x velocity
        float newSpeed = CalculateAccelSpeed(timePosition + playerInput.x * sign);

        // Set rigidbody's velocity
        parent.rb.linearVelocityX = newSpeed * sign;
    }

    private void Decelerate()
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(parent.rb.linearVelocityX);

        // Find current time-position of x velocity
        float timePosition = CurrentDecelTime(parent.rb.linearVelocityX * sign);

        // Find next position of x velocity
        float newSpeed = CalculateDecelSpeed(timePosition + 1);

        // Set rigidbody's velocity
        parent.rb.linearVelocityX = newSpeed * sign;
    }

    private void Jump()
    {
        if (parent.IsGrounded() || parent.coyoteProgress < parent.coyoteTime || parent.exitLadderJump)
        {
            // Set initial vertical velocity
            parent.rb.linearVelocityY = 2 * parent.apexHeight / parent.apexTime;
            // Reset jump progress
            parent.jumpProgress = 0;
            // Ensure coyote time doesn't retrigger
            parent.coyoteProgress += parent.coyoteTime;
        }
    }

    private void NotJumping()
    {
        // Player isn't jumping, so set jumpProgress to "completed" time-value
        parent.jumpProgress = parent.apexTime;

        // Increase coyote time progress
        parent.coyoteProgress += Time.fixedDeltaTime;
        // If player is on the ground, reset coyote time
        if (parent.IsGrounded())
        {
            parent.coyoteProgress = 0;
        }
    }

    private void Fall()
    {
        // If player is still jumping, use ascension gravity
        if (parent.jumpProgress < parent.apexTime)
            parent.rb.linearVelocityY += parent.ascendGravity * Time.fixedDeltaTime;
        // Otherwise use fast-falling gravity
        else
            parent.rb.linearVelocityY += parent.fallGravity * Time.fixedDeltaTime;

        // Increase jump time
        parent.jumpProgress += Time.fixedDeltaTime;

        // Limit falling speed to terminalVelocity
        parent.rb.linearVelocityY = Mathf.Max(parent.rb.linearVelocityY, parent.terminalVelocity);
    }

    private float CurrentAccelTime(float speed)
    {
        return
            speed < parent.maxHorizontalSpeed ?
                speed >= 0 ?
                // Normal acceleration curve
                parent.InverseSimplePolynomial(speed, parent.maxHorizontalSpeed, parent.framesToSpeed, parent.accelModifier) :
                // Turning acceleration curve
                -parent.InverseSimplePolynomial(-speed, parent.maxHorizontalSpeed, Mathf.Min(parent.framesToTurn, parent.framesToStop), parent.turnModifier) :
            parent.framesToSpeed;
    }

    private float CurrentDecelTime(float speed)
    {
        // Deceleration curve
        return
            Mathf.Abs(speed) > 0.05f ?
            -parent.InverseSimplePolynomial(speed, parent.maxHorizontalSpeed, parent.framesToStop, parent.decelModifier) + parent.framesToStop :
            parent.framesToStop;
    }

    private float CalculateAccelSpeed(float time)
    {
        return
            time <= parent.framesToSpeed ?
                time >= 0 ?
                parent.SimplePolynomial(time, parent.maxHorizontalSpeed, parent.framesToSpeed, parent.accelModifier) :
                parent.SimplePolynomial(-time, -parent.maxHorizontalSpeed, Mathf.Min(parent.framesToTurn, parent.framesToStop), parent.turnModifier) :
            parent.maxHorizontalSpeed;
    }

    private float CalculateDecelSpeed(float time)
    {
        return
            time <= parent.framesToStop ?
            parent.SimplePolynomial(-time + parent.framesToStop, parent.maxHorizontalSpeed, parent.framesToStop, parent.decelModifier) :
            0;
    }
}

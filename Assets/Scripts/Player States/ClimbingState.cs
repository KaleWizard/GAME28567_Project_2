using UnityEngine;

public class ClimbingState : BaseState
{
    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
    }
    public override void EnterState()
    {
        parent.rb.linearVelocityY = 0;
    }
    public override void Update(PlayerInput playerInput)
    {
        // Horizontal Movement
        if (playerInput.direction.x != 0)
            AccelerateX(playerInput.direction);
        else
            DecelerateX();

        // Vertical Movement
        if (playerInput.direction.y != 0)
            AccelerateY(playerInput.direction);
        else
            DecelerateY();

        CheckExitState(playerInput);
    }
    public override void ExitState()
    {

    }

    void CheckExitState(PlayerInput playerInput)
    {
        if (parent.IsInLadder()) return;

        // Player has left ladder, exit state
        parent.exitLadderJump = playerInput.direction.y > 0.05f;

        parent.SwapState(parent.BasicMovementState);
    }

    private void AccelerateX(Vector2 playerInput)
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(playerInput.x);

        // Find current time-position of x velocity
        float timePosition = CurrentAccelTime(parent.rb.linearVelocityX * sign, parent.ladderMaxSpeedX);

        // Find next position of x velocity
        float newSpeed = CalculateAccelSpeed(timePosition + playerInput.x * sign, parent.ladderMaxSpeedX);

        // Set rigidbody's velocity
        parent.rb.linearVelocityX = newSpeed * sign;
    }

    private void DecelerateX()
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(parent.rb.linearVelocityX);

        // Find current time-position of x velocity
        float timePosition = CurrentDecelTime(parent.rb.linearVelocityX * sign, parent.ladderMaxSpeedX);

        // Find next position of x velocity
        float newSpeed = CalculateDecelSpeed(timePosition + 1, parent.ladderMaxSpeedX);

        // Set rigidbody's velocity
        parent.rb.linearVelocityX = newSpeed * sign;
    }

    private void AccelerateY(Vector2 playerInput)
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(playerInput.y);

        // Find current time-position of y velocity
        float timePosition = CurrentAccelTime(parent.rb.linearVelocityY * sign, parent.ladderMaxSpeedY);

        // Find next position of y velocity
        float newSpeed = CalculateAccelSpeed(timePosition + playerInput.y * sign, parent.ladderMaxSpeedY);

        // Set rigidbody's velocity
        parent.rb.linearVelocityY = newSpeed * sign;
    }

    private void DecelerateY()
    {
        // Get the direction player wants to move in
        float sign = Mathf.Sign(parent.rb.linearVelocityY);

        // Find current time-position of y velocity
        float timePosition = CurrentDecelTime(parent.rb.linearVelocityY * sign, parent.ladderMaxSpeedY);

        // Find next position of y velocity
        float newSpeed = CalculateDecelSpeed(timePosition + 1, parent.ladderMaxSpeedY);

        // Set rigidbody's velocity
        parent.rb.linearVelocityY = newSpeed * sign;
    }

    private float CurrentAccelTime(float currentSpeed, float maxSpeed)
    {
        return
            currentSpeed < maxSpeed ?
                // Normal acceleration curve
                parent.InverseSimplePolynomial(currentSpeed, maxSpeed, parent.ladderFramesToSpeed, parent.ladderAccelModifier) :
            parent.ladderFramesToSpeed;
    }

    private float CurrentDecelTime(float currentSpeed, float maxSpeed)
    {
        // Deceleration curve
        return
            Mathf.Abs(currentSpeed) > 0.05f ?
            -parent.InverseSimplePolynomial(currentSpeed, maxSpeed, parent.ladderFramesToStop, parent.ladderDecelModifier) + parent.ladderFramesToStop :
            parent.ladderFramesToStop;
    }

    private float CalculateAccelSpeed(float time, float maxSpeed)
    {
        return
            time <= parent.ladderFramesToSpeed ?
                parent.SimplePolynomial(time, maxSpeed, parent.ladderFramesToSpeed, parent.ladderAccelModifier) :
            maxSpeed;
    }

    private float CalculateDecelSpeed(float time, float maxSpeed)
    {
        return
            time <= parent.ladderFramesToStop ?
            parent.SimplePolynomial(-time + parent.ladderFramesToStop, maxSpeed, parent.ladderFramesToStop, parent.ladderDecelModifier) :
            0;
    }
}

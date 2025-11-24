using UnityEngine;

public class DashingState : BaseState
{
    Rigidbody2D rb;

    Vector2 direction;

    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
        rb = parent.rb;
    }
    public override void EnterState()
    {
        rb.linearVelocityY = 0;
        direction = parent.facing == PlayerController.FacingDirection.right ? Vector2.right : Vector2.left; 
    }
    public override void Update(Vector2 playerInput)
    {
        if (Vector2.Dot(playerInput, direction) <= 0) parent.SwapState(parent.BasicMovementState);

        rb.linearVelocity = direction * parent.dashSpeed;
    }
    public override void ExitState()
    {

    }
}

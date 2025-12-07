using UnityEngine;

public class BouncyBallState : BaseState
{
    Rigidbody2D rb;
    PhysicsMaterial2D baseMaterial;

    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
        rb = parent.rb;
        baseMaterial = rb.sharedMaterial;
    }
    public override void EnterState()
    {
        rb.freezeRotation = false;
        rb.sharedMaterial = parent.ballMaterial;

        parent.collider.enabled = false;
        parent.ballCollider.enabled = true;
        
        parent.spriteRenderer.enabled = false;
        parent.ballSpriteRenderer.enabled = true;
    }
    public override void Update(PlayerInput playerInput)
    {
        // Apply gravity
        rb.linearVelocityY += parent.ballTerminalVelocity * Time.fixedDeltaTime / parent.ballTimeToTerminalVelocity;

        // Apply terminal velocity
        rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, parent.ballTerminalVelocity);

        if (playerInput.toBallInput)
            parent.SwapState(parent.BasicMovementState);
    }
    public override void ExitState()
    {
        rb.freezeRotation = true;
        parent.transform.localEulerAngles = Vector3.zero;
        rb.sharedMaterial = parent.ballMaterial;

        parent.collider.enabled = true;
        parent.ballCollider.enabled = false;

        parent.spriteRenderer.enabled = true;
        parent.ballSpriteRenderer.enabled = false;
    }
}


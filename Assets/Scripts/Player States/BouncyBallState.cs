using UnityEngine;

public class BouncyBallState : BaseState
{
    PhysicsMaterial2D baseMaterial;

    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
        baseMaterial = parent.rb.sharedMaterial;
    }
    public override void EnterState()
    {
        Rigidbody2D rb = parent.rb;
        rb.freezeRotation = false;
    }
    public override void Update(PlayerInput playerInput)
    {


        if (playerInput.toBallInput)
            parent.SwapState(parent.BasicMovementState);
    }
    public override void ExitState()
    {

    }
}


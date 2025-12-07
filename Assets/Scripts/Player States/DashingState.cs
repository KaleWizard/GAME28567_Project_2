using UnityEngine;
using UnityEngine.InputSystem;

public class DashingState : BaseState
{
    Rigidbody2D rb;

    Vector2 direction;

    float maxSpeed;

    int frameCount = 0; // Counts frames since dash started

    bool enteredTerrain = false;
    float lastTerrainPosX; // Tracks last x position while the player was in terrain

    int dashHash;

    public override void Initialize(PlayerController parent)
    {
        this.parent = parent;
        rb = parent.rb;
        dashHash = Animator.StringToHash("ToDashing");
    }
    public override void EnterState()
    {
        rb.linearVelocityY = 0;
        direction = parent.facing == PlayerController.FacingDirection.right ? Vector2.right : Vector2.left;
        maxSpeed = parent.dashTotalDist / (parent.dashFramesTotal * Time.fixedDeltaTime);
        frameCount = 0;

        parent.collider.enabled = false;
        enteredTerrain = false;

        parent.animator.SetTrigger(dashHash);
    }
    public override void Update(PlayerInput playerInput)
    {
        UpdateSpeed();
        frameCount++;
        CheckDashEnd(playerInput);
    }
    public override void ExitState()
    {
        parent.collider.enabled = true;
    }

    void UpdateSpeed()
    {
        if (frameCount > parent.dashFramesToSpeed)
        {
            rb.linearVelocity = direction * maxSpeed;
        } else
        {
            rb.linearVelocityX = Mathf.Sign(direction.x) *
                Mathf.Max(
                    Mathf.Abs(rb.linearVelocityX),
                    parent.SimplePolynomial(frameCount, maxSpeed, parent.dashFramesToSpeed, parent.dashAccelModifier));
        }
    }

    void CheckDashEnd(PlayerInput playerInput)
    {
        // Frame count surpassed
        bool frameCountSurpassed = frameCount >= parent.dashFramesToSpeed + parent.dashFramesTotal;

        bool inControlAndNoInput =
            frameCount >= parent.dashFramesToSpeed + parent.dashFramesTotal - parent.dashFramesToControl
            && Vector2.Dot(playerInput.direction, direction) <= 0;

        if (IsInTerrain())
        {
            enteredTerrain = true;
            lastTerrainPosX = parent.transform.position.x;
        }

        bool outOfTerrain = !enteredTerrain || DistanceSince(lastTerrainPosX) >= parent.dashDistAfterTerrain;

        if ((frameCountSurpassed || inControlAndNoInput) && outOfTerrain)
            parent.SwapState(parent.BasicMovementState);
    }

    private float DistanceSince(float startPosition)
    {
        return Mathf.Abs(startPosition - parent.transform.position.x);
    }

    private bool IsInTerrain()
    {
        return parent.terrainCheckCollider.IsTouchingLayers(Physics2D.GetLayerCollisionMask(8)); // Ground layer == 8
    }
}

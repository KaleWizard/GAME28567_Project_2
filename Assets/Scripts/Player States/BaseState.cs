using UnityEngine;

public abstract class BaseState
{
    public PlayerController parent;
    public abstract void Initialize(PlayerController parent);
    public abstract void EnterState();
    public abstract void Update(Vector2 playerInput);
    public abstract void ExitState();
}

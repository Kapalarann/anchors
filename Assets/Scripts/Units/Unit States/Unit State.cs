public abstract class UnitState
{
    public virtual void Enter(UnitStateManager unit) { }
    public virtual void Exit(UnitStateManager unit) { }
    public virtual void FixedUpdate(UnitStateManager unit) { }
}

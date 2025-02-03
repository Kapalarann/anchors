using System;

public class UnitAttackState : UnitState
{
    public override void Enter(UnitStateManager unit) { Console.WriteLine("Entering Attack State"); }
    public override void Update(UnitStateManager unit)
    {
        if (unit._target != null)
        {
            // attack logic
        }
        else
        {
            unit.SetState(unit.idleState);
        }
    }
}


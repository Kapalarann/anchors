using System;

public class UnitIdleState : UnitState
{
    public override void Enter(UnitStateManager unit) { Console.WriteLine("Entering Idle State"); }
    public override void Update(UnitStateManager unit) { Console.WriteLine("Unit is idling"); }
}

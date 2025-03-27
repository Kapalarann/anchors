using System;
using UnityEngine;

public class UnitIdleState : UnitState
{
    public override void Enter(UnitStateManager unit)
    {
        Debug.Log("Entering Idle State");
        unit._animator.SetFloat("Movementspeed", 0f);
    }

    public override void Update(UnitStateManager unit)
    {
        Transform target = unit.AquireTarget();
        if (target != null)
        {
            unit._target = target.GetComponent<UnitStats>();
            unit.SetState(unit.attackState);
            return;
        }
    }
}

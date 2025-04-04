using System;
using UnityEngine;

public class UnitIdleState : UnitState
{
    private float idleTimer;
    public override void Enter(UnitStateManager unit)
    {
        unit._animator.SetFloat("Movementspeed", 0f);
        idleTimer = 0f;
    }

    public override void FixedUpdate(UnitStateManager unit)
    {
        idleTimer += Time.deltaTime;
        if (idleTimer < unit.idleTime) return;

        idleTimer = 0f;

        Transform target = unit.AquireTarget();
        if (target != null)
        {
            unit._target = target.GetComponent<UnitStats>();
            unit.SetState(unit.attackState);
            return;
        }
    }
}

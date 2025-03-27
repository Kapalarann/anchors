using System;
using UnityEngine;

public class UnitAttackState : UnitState
{
    public override void Enter(UnitStateManager unit)
    {
        Debug.Log("Entering Attack State");
    }
    public override void Update(UnitStateManager unit)
    {
        if (unit._target == null)
        {
            unit.SetState(unit.idleState);
            return;
        }

        if (unit.isRanged)
        {
            int i = 0;
            foreach (var attack in unit.rangeAttacks)
            {
                i++;
                if (Vector3.Distance(unit.transform.position, unit._target.transform.position) > attack.maxRange)
                {
                    unit.MoveTo(unit._target.transform.position);
                    return;
                }
                if (attack.attackTimer >= attack.attackCooldown)
                {
                    attack.attackTimer = 0f;
                    unit.currentAttack = i - 1;
                    unit._animator.SetTrigger(attack.animationTrigger);
                    unit.isAttacking = true;
                }
            }
        }

        if (unit.isAttacking)
        {
            Vector3 direction = unit._target.transform.position - unit.transform.position;
            direction.y = 0; // Lock rotation to Y-axis

            if (direction != Vector3.zero) // Prevent errors when target is directly above/below
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, targetRotation, Mathf.Clamp(Time.deltaTime * unit._agent.angularSpeed, 0f, 1f));
            }
        }
    }
}


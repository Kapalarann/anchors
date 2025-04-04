using System;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAttackState : UnitState
{
    private bool wasAttacking = false;

    public override void Enter(UnitStateManager unit)
    {
    }

    public override void FixedUpdate(UnitStateManager unit)
    {
        if (unit._target == null)
        {
            unit.SetState(unit.idleState);
            return;
        }

        if (unit.isRanged) HandleRangedAttack(unit);
        if (unit.isMelee) HandleMeleeAttack(unit);

        if (unit.isAttacking)
        {
            Vector3 direction = unit._target.transform.position - unit.transform.position;
            direction.y = 0; // Lock rotation to Y-axis

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, targetRotation, Mathf.Clamp(Time.deltaTime * unit._agent.angularSpeed, 0f, 1f));
            }
        }

        if (wasAttacking && !unit.isAttacking)
        {
            unit.SetState(unit.idleState);
        }
        wasAttacking = unit.isAttacking;
    }

    private void HandleRangedAttack(UnitStateManager unit)
    {
        foreach (var attack in unit.rangeAttacks)
        {
            if (Vector3.Distance(unit.transform.position, unit._target.transform.position) > attack.maxRange)
            {
                unit.MoveTo(unit._target.transform.position);
                return;
            }
            if (attack.attackTimer >= attack.attackCooldown)
            {
                attack.attackTimer = 0f;
                unit.currentAttack = Array.IndexOf(unit.rangeAttacks, attack);
                unit._animator.SetTrigger(attack.animationTrigger);
                unit.isAttacking = true;
                unit._agent.ResetPath();
            }
        }
    }

    private void HandleMeleeAttack(UnitStateManager unit)
    {
        float dist = Vector3.Distance(unit.transform.position, unit._target.transform.position);

        foreach (var attack in unit.meleeAttacks)
        {
            if (dist > attack.attackRange)
            {
                unit.MoveTo(unit._target.transform.position);
                return;
            }

            if (attack.attackTimer >= attack.attackCooldown)
            {
                attack.attackTimer = 0f;
                unit.currentAttack = Array.IndexOf(unit.meleeAttacks, attack);
                unit._animator.SetTrigger(attack.animationTrigger);
                unit._animator.SetFloat("attackSpeed", attack.attackSpeed);
                unit.isAttacking = true;
                unit._agent.ResetPath();
            }
        }
    }
}

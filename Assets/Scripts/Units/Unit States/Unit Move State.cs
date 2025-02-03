using UnityEngine;

public class UnitMoveState : UnitState
{
    public override void Enter(UnitStateManager unit)
    {
        Debug.Log("Entering Move State");
        unit._navMeshAgent.SetDestination(unit._targetPosition);
    }
    public override void Update(UnitStateManager unit)
    {
        if (!unit._navMeshAgent.pathPending && unit._navMeshAgent.remainingDistance <= unit._navMeshAgent.stoppingDistance)
        {
            unit.SetState(unit.idleState);
        }
    }
}

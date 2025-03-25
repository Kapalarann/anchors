using UnityEngine;

public class UnitMoveState : UnitState
{
    private float noiseOffset;
    public override void Enter(UnitStateManager unit)
    {
        Debug.Log("Entering Move State");
        unit._agent.SetDestination(unit._targetPosition);

        if(unit.strafes) noiseOffset = Random.Range(0f, 100f);
    }
    public override void Update(UnitStateManager unit)
    {
        unit._animator.SetFloat("Movementspeed", unit._agent.velocity.magnitude);

        if (!unit._agent.pathPending && unit._agent.remainingDistance <= unit._agent.stoppingDistance)
        {
            unit._animator.SetFloat("Movementspeed", 0f);
            unit.SetState(unit.idleState);
        }

        if (!unit.strafes || unit._agent.velocity.magnitude <= 0f) return;

        Vector3 toTarget = (unit._targetPosition - unit.transform.position).normalized;
        float strafeAmount = Mathf.PerlinNoise(Time.time * 2f, noiseOffset) * 2f - 1f;
        Vector3 strafeVector = unit.transform.right * strafeAmount * 0.5f * unit.strafeStrength;

        unit._agent.velocity = (toTarget + strafeVector).normalized * unit._agent.speed;
    }

    public override void Exit(UnitStateManager unit)
    {
        unit._agent.velocity = Vector3.zero;
    }
}

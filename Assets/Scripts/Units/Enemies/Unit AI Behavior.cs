using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitBehavior
{
    Melee,
    Range,
    Mage
}

public class UnitAIBehavior : MonoBehaviour
{
    private UnitStateManager _unit;
    private float decisionTimer = 0f;

    [SerializeField] private float decisionInterval = 1.5f;
    public UnitType behaviorProfile;

    private void Start()
    {
        _unit = GetComponent<UnitStateManager>();
        decisionTimer = decisionInterval;
    }

    private void Update()
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            decisionTimer = decisionInterval;
            MakeDecision();
        }
    }

    private void MakeDecision()
    {
        if (behaviorProfile == null || _unit == null) return;

        float totalWeight = behaviorProfile.GetTotalWeight();
        if (totalWeight <= 0)
        {
            Debug.LogWarning($"{gameObject.name} has invalid AI weights!");
            return;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0;

        if (randomValue <= (cumulative += behaviorProfile.idleWeight))
        {
            _unit.SetState(_unit.idleState);
        }
        else if (randomValue <= (cumulative += behaviorProfile.moveWeight))
        {
            _unit.SetState(_unit.moveState);
        }
        else
        {
            _unit.SetState(_unit.attackState);
        }
    }
}
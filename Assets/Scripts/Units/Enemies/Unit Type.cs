using UnityEngine;

[CreateAssetMenu(fileName = "UnitType", menuName = "Scriptable Objects/UnitType")]
public class UnitType : ScriptableObject
{
    public UnitBehavior unitType;

    [Header("Decision Weights")]
    [Range(0f, 1f)] public float moveWeight = 0.5f;
    [Range(0f, 1f)] public float attackWeight = 0.5f;
    [Range(0f, 1f)] public float idleWeight = 0.1f;

    public float GetTotalWeight()
    {
        return moveWeight + attackWeight + idleWeight;
    }
}
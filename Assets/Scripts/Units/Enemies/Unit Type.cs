using UnityEngine;

[CreateAssetMenu(fileName = "UnitType", menuName = "Scriptable Objects/UnitType")]
public class UnitType : ScriptableObject
{
    public UnitBehavior unitType;

    [Header("Decision Weights")]
    [Range(0f, 1f)] public float moveWeight = 0.5f;
    [Range(0f, 1f)] public float attackWeight = 0.5f;
    [Range(0f, 1f)] public float idleWeight = 0.1f;
    [Range(0f, 1f)] public float cast1Weight = 0f;
    [Range(0f, 1f)] public float cast2Weight = 0f;
    [Range(0f, 1f)] public float cast3Weight = 0f;

    public float GetTotalWeight()
    {
        return moveWeight + attackWeight + idleWeight;
    }
}
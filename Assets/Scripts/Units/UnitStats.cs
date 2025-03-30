using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public static HashSet<UnitStats> units = new HashSet<UnitStats>();

    public string unitName;
    public StateType unitType;

    private void Awake()
    {
        units.Add(this);
    }

    private void OnDestroy()
    {
        units.Remove(this);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public static HashSet<UnitStats> units = new HashSet<UnitStats>();
    private static UnitStats[] unitArray;

    public string unitName;
    public StateType unitType;

    private void Awake()
    {
        units.Add(this);
        UpdateUnitArray();
    }

    private void OnDestroy()
    {
        units.Remove(this);
        UpdateUnitArray();
    }

    public static UnitStats[] GetUnitsArray()
    {
        return unitArray;
    }

    private static void UpdateUnitArray()
    {
        unitArray = units.ToArray();
    }
}

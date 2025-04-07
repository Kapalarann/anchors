using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public static HashSet<UnitStats> units = new HashSet<UnitStats>();
    private static UnitStats[] unitArray;
    [SerializeField] private NameList nameList;

    public string unitName;
    public StateType unitType;

    private void Awake()
    {
        units.Add(this);
        UpdateUnitArray();
        PickRandomName();
    }

    private void OnDestroy()
    {
        units.Remove(this);
        UpdateUnitArray();
    }

    void PickRandomName()
    {
        if (nameList == null || nameList.names.Length == 0)
        {
            Debug.LogWarning("Name list is empty or missing!");
            return;
        }

        int index = Random.Range(0, nameList.names.Length);
        unitName = nameList.names[index];
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

[CreateAssetMenu(fileName = "NameList", menuName = "Custom/Name List")]
public class NameList : ScriptableObject
{
    public string[] names;
}
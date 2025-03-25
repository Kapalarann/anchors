using System.Collections;
using UnityEngine;

public class ESpell : Spell
{
    private Transform currentUnit;
    private GameObject[] allUnits;

    private void Start()
    {
        allUnits = GameObject.FindGameObjectsWithTag("Unit");
        currentUnit = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (currentUnit == null)
        {
            Debug.LogError("❌ No active player found! Ensure a unit is tagged as 'Player'.");
        }
    }

    public override void Cast(Vector3 targetPosition)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject selectedUnit = hit.collider.gameObject;

            if (selectedUnit.CompareTag("Unit"))
            {
                SwitchUnit(selectedUnit);
            }
            else
            {
                Debug.Log("No unit selected.");
            }
        }
    }

    private void SwitchUnit(GameObject newUnit)
    {
        if (newUnit == null || currentUnit == newUnit.transform)
        {
            Debug.Log("❌ Invalid unit switch attempt.");
            return;
        }

        
        currentUnit.tag = "Unit";
        currentUnit.GetComponent<PlayerController>().enabled = false;
        currentUnit.GetComponent<SpellCaster>().enabled = false;

        
        newUnit.tag = "Player";
        newUnit.GetComponent<PlayerController>().enabled = true;
        newUnit.GetComponent<SpellCaster>().enabled = true;

        
        currentUnit = newUnit.transform;

        Debug.Log($"🔄 Switched control to {newUnit.name}");
    }
}

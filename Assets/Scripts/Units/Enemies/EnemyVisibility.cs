using UnityEngine;
using System.Collections.Generic;

public class EnemyVisibility : MonoBehaviour
{
    private Renderer[] renderers;
    private HashSet<GameObject> visibleToUnits; // Keeps track of units that can see this enemy

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        visibleToUnits = new HashSet<GameObject>();
    }

    public void AddViewer(GameObject unit)
    {
        if (visibleToUnits.Add(unit)) // Add unit to the set
        {
            Reveal();
        }
    }

    public void RemoveViewer(GameObject unit)
    {
        if (visibleToUnits.Remove(unit) && visibleToUnits.Count == 0) // Remove unit and check if empty
        {
            Hide();
        }
    }

    private void SetRenderers(bool state)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = state;
        }
    }

    public void Reveal()
    {
        SetRenderers(true); // Make enemy visible
    }

    public void Hide()
    {
        SetRenderers(false); // Make enemy hidden
    }
}

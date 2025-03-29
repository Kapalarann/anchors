using UnityEngine;
using UnityEngine.AI;

public class SelectableUnit : MonoBehaviour
{
    private NavMeshAgent agent;
    private Renderer unitRenderer;
    private Color originalColor;

    private UnitStats stats;
    private Health health;
    public MonoBehaviour movementScript;

    void Start()
    {
        unitRenderer = GetComponent<Renderer>();
        stats = GetComponent<UnitStats>();
        health = GetComponent<Health>();

        originalColor = unitRenderer.material.color;
    }

    public void OnSelect()
    {
        unitRenderer.material.color = Color.green;
        StatsUI.Instance.UpdateStats(stats, health);
    }

    public void OnDeselect()
    {
        unitRenderer.material.color = originalColor;
        StatsUI.Instance.ClearStats();
    }
}

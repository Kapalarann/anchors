using UnityEditor;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    private Renderer unitRenderer;
    private Color originalColor;

    private UnitStats stats;
    public MonoBehaviour movementScript;

    void Start()
    {
        unitRenderer = GetComponent<Renderer>();
        stats = GetComponent<UnitStats>();

        originalColor = unitRenderer.material.color;
    }

    public void OnSelect()
    {
        unitRenderer.material.color = Color.green;
        StatsUI.Instance.UpdateStats(stats);
    }

    public void OnDeselect()
    {
        unitRenderer.material.color = originalColor;
        StatsUI.Instance.ClearStats();
    }
}

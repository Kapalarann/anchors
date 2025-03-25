using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public static StatsUI Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitTypeText;
    public TextMeshProUGUI healthText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateStats(UnitStats stats, Health hp)
    {
        if (stats == null) return;

        unitNameText.text = $"Name: {stats.unitName}";
        unitTypeText.text = $"Type: {stats.unitType}";
        healthText.text = $"Health: {hp.HP}/{hp.maxHP}";
    }

    public void ClearStats()
    {
        unitNameText.text = "";
        unitTypeText.text = "";
        healthText.text = "";
    }
}

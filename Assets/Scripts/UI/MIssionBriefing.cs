using UnityEngine;
using UnityEngine.UI;

public class MissionBriefing : MonoBehaviour
{
    public GameObject briefingPanel;
    public Button startMissionButton;
    public Button cancelButton;

    [SerializeField] private GameObject healthBarContainer;

    private void Start()
    {
        briefingPanel.SetActive(true);
        Time.timeScale = 0;

        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(false); // Hide health bar during briefing
        }

        // Listeners
        startMissionButton.onClick.AddListener(StartMission);
        cancelButton.onClick.AddListener(CancelMission);
    }

    public void StartMission()
    {
        briefingPanel.SetActive(false);
        Time.timeScale = 1;

        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(true); // Show health bar when mission starts
        }
    }

    public void CancelMission()
    {
        briefingPanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Mission canceled.");
    }
}

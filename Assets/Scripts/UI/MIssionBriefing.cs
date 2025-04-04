using UnityEngine;
using UnityEngine.UI;

public class MissionBriefing : MonoBehaviour
{
    public GameObject briefingPanel;
    public Button startMissionButton;
    public Button cancelButton;

    private void Start()
    {
        briefingPanel.SetActive(true);
        Time.timeScale = 0; 

        //Listeners
        startMissionButton.onClick.AddListener(StartMission);
        cancelButton.onClick.AddListener(CancelMission);
    }

    public void StartMission()
    {
        briefingPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void CancelMission()
    {
        briefingPanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Mission canceled.");
    }
}

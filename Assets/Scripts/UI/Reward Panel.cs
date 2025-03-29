using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Needed if you want to load another scene

public class RewardPanel : MonoBehaviour
{
    public GameObject panel;
    public Button continueButton;
    public Button closeButton;

    void Start()
    {
        panel.SetActive(false);  // Hide panel at start
        continueButton.onClick.AddListener(ContinueGame);
        closeButton.onClick.AddListener(HidePanel);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);  // Show panel
    }

    public void HidePanel()
    {
        panel.SetActive(false);  // Hide panel when clicked
    }

    public void ContinueGame()
    {
        // If you have levels, load next scene (adjust scene index)
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Or just hide panel and continue gameplay
        panel.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //incase of another scene

public class RewardPanel : MonoBehaviour
{
    public GameObject panel;
    public Button continueButton;
    public Button closeButton;

    void Start()
    {
        panel.SetActive(false); //Hides panel
        continueButton.onClick.AddListener(ContinueGame);
        closeButton.onClick.AddListener(HidePanel);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void ContinueGame()
    {
        panel.SetActive(false);
    }
}

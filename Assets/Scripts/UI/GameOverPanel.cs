using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public GameObject panel;
    public Button restartButton;
    public Button exitButton;

    void Start()
    {
        panel.SetActive(false); //Hides Panel
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitToMainMenu);
    }

    public void ShowGameOverPanel()
    {
        panel.SetActive(true);  // Show panel when the player loses
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  //Reload Scene
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  //Directs you to Main Menu
    }
}

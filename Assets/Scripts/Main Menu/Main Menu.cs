using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    //void Start()
    //{
    //    startButton.onClick.AddListener(StartGame);
    //    optionsButton.onClick.AddListener(OptionMenu);
    //    quitButton.onClick.AddListener(QuitGame);
    //}

    //void StartGame()
    //{
    //    SceneManager.LoadScene("GameScene");
    //    Debug.Log("Loading game scene.");
    //}

    //void OptionMenu()
    //{
    //    Debug.Log("Option menu clicked.");
    //}

    //void QuitGame()
    //{
    //    Debug.Log("Quitting game.");
    //}
    void Start()
    {
        startButton.onClick.AddListener(() => Debug.Log("Start button clicked."));
        optionsButton.onClick.AddListener(() => Debug.Log("Option button clicked"));
        quitButton.onClick.AddListener(() => Debug.Log("Quit button clicked"));
    }
}

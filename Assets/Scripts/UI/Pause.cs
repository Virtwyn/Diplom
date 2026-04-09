using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] private string menuSceneName = "Menu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    public void pause()
    {
        PauseGame();
    }
    public void continuegame()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
    public void TogglePause()
    {
        if (panel == null)
        {
            return;
        }

        if (panel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void OpenMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(menuSceneName) && Application.CanStreamedLevelBeLoaded(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
            return;
        }

        SceneManager.LoadScene(0);
    }
    public void menu()
    {
        OpenMenu();
    }
    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}

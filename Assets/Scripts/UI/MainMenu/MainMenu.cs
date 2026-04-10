using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        //Загрузка первого уровня
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //Востановление времени
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
        Debug.Log("Закрыто");
        // Закрыть игру
        Application.Quit();
    }
}

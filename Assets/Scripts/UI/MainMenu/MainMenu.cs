using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        // Старт: переходим на следующую сцену после меню.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // Гарантируем обычное время игры при запуске уровня.
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
        Debug.Log("Закрыто");
        // Закрытие приложения (в Editor визуально не срабатывает).
        Application.Quit();
    }
}

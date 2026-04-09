using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    // Панель паузы (UI), которую показываем/скрываем.
    public GameObject panel;
    // Имя сцены главного меню из Build Settings.
    [SerializeField] private string menuSceneName = "Menu";

    private void Update()
    {
        // По Esc переключаем состояние паузы.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    // Старое имя метода для кнопки "Pause" (совместимость с OnClick).
    public void pause()
    {
        PauseGame();
    }
    // Старое имя метода для кнопки "Continue" (совместимость с OnClick).
    public void continuegame()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        // Останавливаем игру и показываем меню паузы.
        panel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        // Возобновляем игру и скрываем меню паузы.
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void TogglePause()
    {
        // Если панель активна -> продолжить, иначе -> поставить на паузу.
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
        // Перед сменой сцены обязательно возвращаем нормальное время.
        Time.timeScale = 1f;

        // Если сцена по имени найдена, открываем ее.
        if (!string.IsNullOrWhiteSpace(menuSceneName) && Application.CanStreamedLevelBeLoaded(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
            return;
        }

        // Иначе запасной вариант: первая сцена в Build Settings.
        SceneManager.LoadScene(0);
    }
    // Старое имя метода для кнопки "Menu" (совместимость с OnClick).
    public void menu()
    {
        OpenMenu();
    }
    public void ExitGame()
    {
        // На всякий случай сбрасываем timeScale перед выходом.
        Time.timeScale = 1f;
        Application.Quit();
    }
}

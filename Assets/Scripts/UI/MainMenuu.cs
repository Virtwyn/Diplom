using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    // Ссылка на главный контроллер паузы.
    [SerializeField] private Pause pauseController;

    private void Awake()
    {
        // Если не задано в Inspector, пробуем найти автоматически.
        if (pauseController == null)
        {
            pauseController = FindFirstObjectByType<Pause>();
        }
    }

    // Старый обработчик кнопки "Menu".
    public void menu()
    {
        // Делегируем действие в Pause, чтобы логика была в одном месте.
        pauseController.OpenMenu();
    }
}

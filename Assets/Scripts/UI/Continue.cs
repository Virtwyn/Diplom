using UnityEngine;

public class Continue : MonoBehaviour
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

    // Старый обработчик кнопки "Continue".
    public void continuegame() 
    {
        // Делегируем действие в Pause, чтобы логика была в одном месте.
        pauseController.ResumeGame();
    }
}

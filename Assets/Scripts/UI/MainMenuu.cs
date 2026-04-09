using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private Pause pauseController;

    private void Awake()
    {
        if (pauseController == null)
        {
            pauseController = FindFirstObjectByType<Pause>();
        }
    }

    public void menu()
    {
        if (pauseController != null)
        {
            pauseController.OpenMenu();
            return;
        }

        Time.timeScale = 1f;
    }
}

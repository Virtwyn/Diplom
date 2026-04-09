using UnityEngine;

public class Continue : MonoBehaviour
{
    [SerializeField] private Pause pauseController;

    private void Awake()
    {
        if (pauseController == null)
        {
            pauseController = FindFirstObjectByType<Pause>();
        }
    }

    public void continuegame() 
    {
        if (pauseController != null)
        {
            pauseController.ResumeGame();
            return;
        }

        Time.timeScale = 1f;
    }
}

using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [Header("UI элементы")]
    [Tooltip("TextMeshPro текст — будет показывать 'X3'")]
    [SerializeField] private TextMeshProUGUI livesText;

    private void Start()
    {
        UpdateLivesUI();
    }

    public void UpdateLivesUI()
    {
        if (LivesSystem.Instance == null) return;

        int current = LivesSystem.Instance.GetCurrentLives();
        livesText.text = $"X0{current}";
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesSystem : MonoBehaviour
{
    public static LivesSystem Instance;

    [SerializeField] private int maxLives = 3;
    private int _currentLives;

    private const string KEY_LIVES = "current_lives";

    // Ссылка на UI (найдётся автоматически через FindObjectOfType)
    private LivesUI _livesUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Загружаем сохранённые жизни, если есть
            _currentLives = PlayerPrefs.GetInt(KEY_LIVES, maxLives);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshUIReference();
    }

    // Переподключаемся к UI после смены сцены
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUIReference();
    }

    private void RefreshUIReference()
    {
        _livesUI = FindObjectOfType<LivesUI>();
        if (_livesUI != null)
            _livesUI.UpdateLivesUI();
    }

    public void LoseLife()
    {
        _currentLives--;

        if (_livesUI != null)
            _livesUI.UpdateLivesUI();

        if (_currentLives <= 0)
        {
            _currentLives = maxLives;
            PlayerPrefs.SetInt(KEY_LIVES, _currentLives);
            PlayerPrefs.Save();
            CheckpointManager.DeleteSave();
            CheckpointManager.Instance?.ResetCheckpoint();
            SceneManager.LoadScene("НазваниеПервойСцены");
        }
        else
        {
            // Сохраняем жизни после каждой смерти
            PlayerPrefs.SetInt(KEY_LIVES, _currentLives);
            PlayerPrefs.Save();
        }
    }

    public void ResetLives()
    {
        _currentLives = maxLives;
        PlayerPrefs.SetInt(KEY_LIVES, _currentLives);
        PlayerPrefs.Save();

        if (_livesUI != null)
            _livesUI.UpdateLivesUI();
    }


    public int GetCurrentLives() => _currentLives;
    public int GetMaxLives() => maxLives;
}

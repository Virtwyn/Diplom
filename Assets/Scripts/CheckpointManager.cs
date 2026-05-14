using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Настройки")]
    public Checkpoint defaultCheckpoint;
    public bool resetOnSceneLoad = true;

    private Checkpoint _lastCheckpoint;
    public Checkpoint LastCheckpoint => _lastCheckpoint;

    private const string KEY_CHECKPOINT = "last_checkpoint_name";
    private const string KEY_SCENE = "last_checkpoint_scene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (defaultCheckpoint != null)
            _lastCheckpoint = defaultCheckpoint;
    }

    public void SetLastCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null) return;

        _lastCheckpoint = checkpoint;

        // Сохраняем имя чекпоинта и текущую сцену
        PlayerPrefs.SetString(KEY_CHECKPOINT, checkpoint.checkpointName);
        PlayerPrefs.SetString(KEY_SCENE, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

    }

    public Vector3 GetRespawnPosition()
    {
        if (_lastCheckpoint != null)
            return _lastCheckpoint.transform.position;
        return Vector3.zero;
    }

    public void ResetCheckpoint()
    {
        if (resetOnSceneLoad && defaultCheckpoint != null)
            _lastCheckpoint = defaultCheckpoint;
    }

    // Есть ли сохранённая игра?
    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(KEY_SCENE);
    }

    // Загрузить последнюю сохранённую сцену
    public static void LoadLastScene()
    {
        if (!HasSavedGame()) return;
        string sceneName = PlayerPrefs.GetString(KEY_SCENE);
        SceneManager.LoadScene(sceneName);
    }

    // Получить имя сохранённого чекпоинта (нужно после загрузки сцены)
    public static string GetSavedCheckpointName()
    {
        return PlayerPrefs.GetString(KEY_CHECKPOINT, "");
    }

    // Удалить сохранение (например при смерти всех жизней)
    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(KEY_CHECKPOINT);
        PlayerPrefs.DeleteKey(KEY_SCENE);
        PlayerPrefs.Save();
    }
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
        // Ищем чекпоинт по сохранённому имени
        string savedName = GetSavedCheckpointName();
        if (string.IsNullOrEmpty(savedName)) return;

        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp.checkpointName == savedName)
            {
                _lastCheckpoint = cp;
                // Телепортируем игрока
                TeleportPlayerToCheckpoint();
                return;
            }
        }
    }

    private void TeleportPlayerToCheckpoint()
    {
        // Ищем игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && _lastCheckpoint != null)
        {
            player.transform.position = _lastCheckpoint.transform.position;
        }
    }
}
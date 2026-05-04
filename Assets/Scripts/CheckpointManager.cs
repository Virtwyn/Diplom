using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // 🔧 Синглтон: доступ отовсюду
    public static CheckpointManager Instance { get; private set; }

    [Header("Настройки")]
    [Tooltip("Точка по умолчанию (начало уровня)")]
    public Checkpoint defaultCheckpoint;

    [Tooltip("Сбрасывать точки при загрузке сцены?")]
    public bool resetOnSceneLoad = true;

    // Последняя активная точка
    private Checkpoint _lastCheckpoint;
    public Checkpoint LastCheckpoint => _lastCheckpoint;

    private void Awake()
    {
        // Создаём синглтон
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Не удалять при загрузке сцены
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Устанавливаем точку по умолчанию
        if (defaultCheckpoint != null)
            _lastCheckpoint = defaultCheckpoint;
    }

    // Установить последнюю точку
    public void SetLastCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint != null)
        {
            _lastCheckpoint = checkpoint;
            Debug.Log($"📍 Новая точка сохранения: {checkpoint.checkpointName}");
        }
    }

    // Получить позицию для респавна
    public Vector3 GetRespawnPosition()
    {
        if (_lastCheckpoint != null)
            return _lastCheckpoint.transform.position;

        Debug.LogWarning("⚠️ Нет активной точки, используем (0,0,0)");
        return Vector3.zero;
    }

    // Сброс точек (при смерти или загрузке)
    public void ResetCheckpoint()
    {
        if (resetOnSceneLoad && defaultCheckpoint != null)
        {
            _lastCheckpoint = defaultCheckpoint;
            Debug.Log("🔄 Точки сброшены к умолчанию");
        }
    }
}
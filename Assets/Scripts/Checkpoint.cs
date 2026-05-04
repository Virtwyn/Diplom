using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Настройки точки")]
    [Tooltip("Уникальное имя для отладки")]
    public string checkpointName = "Checkpoint";

    [Tooltip("Восстанавливать здоровье при респавне?")]
    public bool restoreHealth = true;

    [Tooltip("Сколько здоровья восстановить (0-100%)")]
    [Range(0, 100)] public int healPercent = 100;

    [Header("Визуал (опционально)")]
    [Tooltip("Объект-визуал точки (флаг, костёр и т.д.)")]
    public GameObject visualEffect;

    [Tooltip("Эффект при активации (частицы, звук)")]
    public GameObject activateEffect;

    // Цвет для отладки в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
        Gizmos.color = Color.white;
    }

    // При входе игрока в триггер
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint(other.GetComponent<HealthSystem>());
        }
    }

    // Активация точки
    public void ActivateCheckpoint(HealthSystem player)
    {
        if (player == null) return;

        // Сохраняем эту точку как последнюю
        CheckpointManager.Instance.SetLastCheckpoint(this);

        Debug.Log($"✅ Активирована точка: {checkpointName}");

        // Визуальные эффекты
        if (visualEffect != null)
            visualEffect.SetActive(true);

        if (activateEffect != null)
        {
            Instantiate(activateEffect, transform.position, Quaternion.identity);
        }

        // Лечение если настроено
        //if (restoreHealth && player != null)
        //{
        //    int healAmount = Mathf.RoundToInt(player.maxHealth * (healPercent / 100f));
        //    player.Heal(healAmount);
        //    Debug.Log($"💚 Восстановлено {healAmount} здоровья");
        //}
    }
}
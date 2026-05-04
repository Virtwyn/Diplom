using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Требования")]
    [SerializeField] private int keysRequired = 1;
    [SerializeField] private bool consumeKeysOnOpen = true;

    [Header("Ссылки")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Триггерная зона проверки ключа (обычно чуть больше двери).")]
    [SerializeField] private Collider2D keyCheckTrigger;
    [Tooltip("Физический коллайдер двери (не trigger), который блокирует проход.")]
    [SerializeField] private Collider2D blockingCollider;

    [Header("Анимация")]
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private bool disableColliderOnOpen = true;

    private bool _isOpened;

    private void Awake()
    {
        if (keyCheckTrigger == null)
        {
            keyCheckTrigger = GetComponent<Collider2D>();
        }

        if (blockingCollider == null)
        {
            blockingCollider = GetComponent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryOpenBy(other);
    }

    private void TryOpenBy(Collider2D other)
    {
        if (_isOpened || other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (KeyCount.Instance == null)
        {
            Debug.LogWarning("KeyCount не найден на сцене.");
            return;
        }

        bool hasEnoughKeys = KeyCount.Instance.HasKeys(keysRequired);
        if (!hasEnoughKeys)
        {
            return;
        }

        if (consumeKeysOnOpen && !KeyCount.Instance.TrySpendKeys(keysRequired))
        {
            return;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        _isOpened = true;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);
        }

        if (disableColliderOnOpen && blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }
    }
}

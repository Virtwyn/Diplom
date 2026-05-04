using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private int keyAmount = 1;
    [SerializeField] private AudioClip pickupSound;

    private bool _isCollected;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isCollected || !other.CompareTag("Player"))
        {
            return;
        }

        if (KeyCount.Instance == null)
        {
            Debug.LogWarning("KeyCount не найден на сцене.");
            return;
        }

        _isCollected = true;
        KeyCount.Instance.AddKey(keyAmount);

        if (pickupSound != null && SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(pickupSound, transform, 1f);
        }

        Destroy(gameObject);
    }
}

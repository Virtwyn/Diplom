using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    [Tooltip("Если включено, урон повторяется с интервалом, пока объект касается шипов.")]
    [SerializeField] private bool damageWhileTouching = true;

    [SerializeField] private float damageInterval = 1f;

    private readonly Dictionary<int, float> _nextDamageTime = new Dictionary<int, float>();

    void OnTriggerEnter2D(Collider2D other)
    {
        ApplyDamage(other, fromStay: false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (damageWhileTouching)
            ApplyDamage(other, fromStay: true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _nextDamageTime.Remove(other.GetInstanceID());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ApplyDamage(collision.collider, fromStay: false);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (damageWhileTouching)
            ApplyDamage(collision.collider, fromStay: true);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _nextDamageTime.Remove(collision.collider.GetInstanceID());
    }

    void ApplyDamage(Collider2D other, bool fromStay)
    {
        if (other == null) return;

        int id = other.GetInstanceID();
        float now = Time.time;

        if (damageWhileTouching)
        {
            if (fromStay && _nextDamageTime.TryGetValue(id, out float next) && now < next)
                return;
        }

        bool dealt = false;

        HealthSystem health = other.GetComponent<HealthSystem>() ?? other.GetComponentInParent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage, transform);
            dealt = true;
        }

        EnemyCombat enemy = other.GetComponent<EnemyCombat>() ?? other.GetComponentInParent<EnemyCombat>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            dealt = true;
        }

        if (dealt && damageWhileTouching)
            _nextDamageTime[id] = now + damageInterval;
    }
}

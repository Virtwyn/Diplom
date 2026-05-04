using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float damageInterval = 0.5f;
    private float _nextTickTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Сбрасываем таймер и наносим урон сразу при входе
        _nextTickTime = 0f;
        DealDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time >= _nextTickTime)
        {
            DealDamage(other);
            _nextTickTime = Time.time + damageInterval;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _nextTickTime = 0f;
        DealDamage(collision.collider);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time >= _nextTickTime)
        {
            DealDamage(collision.collider);
            _nextTickTime = Time.time + damageInterval;
        }
    }

    private void DealDamage(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health == null) health = other.GetComponentInParent<HealthSystem>();
            health?.TakeDamage(damage, transform);
        }

        if (other.CompareTag("Enemy"))
        {
            EnemyCombat enemy = other.GetComponent<EnemyCombat>();
            if (enemy == null) enemy = other.GetComponentInParent<EnemyCombat>();
            enemy?.TakeDamage(damage);
        }
    }
}
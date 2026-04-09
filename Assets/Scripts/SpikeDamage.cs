using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    private bool damageWhileTouching = false;
    [SerializeField] private float damageInterval = 0.5f;
    private float _nextTickTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        DealDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (damageWhileTouching && Time.time >= _nextTickTime)
        {
            DealDamage(other);
            _nextTickTime = Time.time + damageInterval;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision.collider);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (damageWhileTouching && Time.time >= _nextTickTime)
        {
            DealDamage(collision.collider);
            _nextTickTime = Time.time + damageInterval;
        }
    }

    private void DealDamage(Collider2D other)
    {
        // Проверяем тег Player
        if (other.CompareTag("Player"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health == null) health = other.GetComponentInParent<HealthSystem>();
            health.TakeDamage(damage, transform);
        }

        // Проверяем тег Enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyCombat enemy = other.GetComponent<EnemyCombat>();
            if (enemy == null) enemy = other.GetComponentInParent<EnemyCombat>();
            enemy.TakeDamage(damage);
        }
    }
}

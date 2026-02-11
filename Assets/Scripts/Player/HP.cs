using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            TakeDamage(1);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            float alpha = Mathf.PingPong(Time.time * 5f, 1f);
            _spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        if (transform.position.y < -15)
            Die();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
            }
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
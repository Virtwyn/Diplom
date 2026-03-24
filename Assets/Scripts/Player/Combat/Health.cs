using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Визуал")]
    public Slider healthSlider;
    public TextMeshProUGUI healthBarValueText;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Показатели")]
    public int maxHealth;
    public int currentHealth;

    [SerializeField] private float invincibilityTime = 1;
    private bool isInvincible;
    private float invincibilityTimer;
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
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
        if (transform.position.y < -95)
            Die();
        healthBarValueText.text = currentHealth.ToString()+ "/" + maxHealth.ToString();
        healthSlider.value=currentHealth;
        healthSlider.maxValue = maxHealth; 
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

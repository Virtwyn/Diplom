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
    public Animator anim;
    public SpriteRenderer _characterSprite;
    Vector2 startPos;
    public Rigidbody2D rbPlayer;

    [Header("Показатели здоровья")]
    public int maxHealth;
    public int currentHealth;

    [SerializeField] private float invincibilityTime = 1;
    private bool isInvincible;
    private float invincibilityTimer;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    void Update()
    {
        rbPlayer.WakeUp();
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
        if (transform.position.y < -12)
            Respawn();
        healthBarValueText.text = currentHealth.ToString()+ "/" + maxHealth.ToString();
        healthSlider.value = currentHealth;
        healthSlider.maxValue = maxHealth; 
    }
    //Получение урона
    public void TakeDamage(int damage, Transform attacker)
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
    //Смерть игрока
    private void Die()
    {
        anim.Play("Death");
        GetComponent<CharacterMovement>().enabled=false;
        GetComponent<PlayerCombat>().enabled = false;
        gameObject.tag = "Untagged";
        Invoke("Respawn", 2f);
    }
    // Перезагрузка после смерти игрока
    void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

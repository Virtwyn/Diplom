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
    private PlayerShield _playerShield;
    private bool _suppressBlinking = false;


    [Header("Респавн")]
    public bool resetHealthOnRespawn = true;
    [SerializeField] private Vector3 _defaultRespawnPos;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        _playerShield = GetComponent<PlayerShield>();
    }
    void Update()
    {
        rbPlayer.WakeUp();
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (!_suppressBlinking)
            {
                float alpha = Mathf.PingPong(Time.time * 5f, 1f);
                _spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                _suppressBlinking = false;
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

        int finalDamage = damage;
        if (_playerShield != null)
        {
            finalDamage = _playerShield.GetModifiedDamage(damage, attacker);
        }

        if (finalDamage <= 0)
        {
            return;
        }

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.Play("Hurt");
            isInvincible = true;
            invincibilityTimer = invincibilityTime;
        }
    }
    //Смерть игрока
    private void Die()
    {
        anim.Play("Death");
        LivesSystem.Instance.LoseLife();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;

        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        gameObject.tag = "Untagged";
        Invoke("Respawn", 2f);
    }
    // Перезагрузка после смерти игрока
    void Respawn()
    {
        if (LivesSystem.Instance != null && LivesSystem.Instance.GetCurrentLives() <= 0)
            return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector3 respawnPos = CheckpointManager.Instance != null
            ? CheckpointManager.Instance.GetRespawnPosition()
            : _defaultRespawnPos;
        if (resetHealthOnRespawn)
        {
            currentHealth = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth;
            healthBarValueText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
        CharacterMovement movement = GetComponent<CharacterMovement>();
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (movement != null) movement.enabled = true;
        if (combat != null) combat.enabled = true;
        gameObject.tag = "Player";
        transform.position = respawnPos;
        rbPlayer.linearVelocity = Vector2.zero; 
        rbPlayer.angularVelocity = 0f;
        if (anim != null)
        {
            anim.Play("Idle");
        }
        isInvincible = true;
        invincibilityTimer = invincibilityTime;
    }
    public void SetInvincible(float duration, bool suppressBlinking = false)
    {
        isInvincible = true;
        invincibilityTimer = duration;
        _suppressBlinking = suppressBlinking;
    }
}


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

    [Header("Показатели")]
    public int maxHealth;
    public int currentHealth;

    [SerializeField] private float invincibilityTime = 1;
    private bool isInvincible;
    private float invincibilityTimer;

    [Header("Щит")]
    private bool block = true;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    void Update()
    {
        //Block();
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
    private void Die()
    {
        anim.Play("Death");
        GetComponent<CharacterMovement>().enabled=false;
        GetComponent<PlayerCombat>().enabled = false;
        gameObject.tag = "Untagged";
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    bool IsAttackFromFront(Transform enemy)
    {
        int playerLookDirection = _characterSprite.flipX ? -1 : 1;
        float directionToEnemy = Mathf.Sign(enemy.position.x - transform.position.x);
        return playerLookDirection == directionToEnemy;
    }
    //void Block()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        anim.Play("Block");
    //        block = true;
    //    }
    //}
}

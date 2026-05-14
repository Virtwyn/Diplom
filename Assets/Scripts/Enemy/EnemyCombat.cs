using UnityEngine;
using UnityEngine.Windows;

public class EnemyCombat : MonoBehaviour
{
    [Header("Боёвка врага")]
    public int hp;
    public int damage;
    private Transform player;
    [SerializeField] private Animator anim;
    private Vector3 _attackOffset;
    [SerializeField] private SpriteRenderer _enemySprite;
    public float recharge;
    public float startRecharge;
    public float radius;
    [SerializeField] private float attackLockDuration = 0.6f;
    [Header("Передвижние врага")]
    public Enemy speed;

    private float invincibilityTime=0.05f;
    private bool isInvincible;
    private float invincibilityTimer;

    [Header("Настройка боёвки")]
    public Collider2D enemyCollider;
    public Transform attackPos;
    public LayerMask playerMask;
    private bool playerInRange;
    private bool isAttacking;
    private float defaultSpeed;
    
    void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        _enemySprite = GetComponentInChildren<SpriteRenderer>();
        _attackOffset = attackPos.localPosition;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        defaultSpeed = speed.speed;
    }

    void Update()
    {
        UpdateAttackPointPosition();
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            float alpha = Mathf.PingPong(Time.time * 5f, 1f);
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
        if (playerInRange)
        {
            speed.speed = 0;
            anim.SetBool("Run", false);

            if (!isAttacking)
            {
                recharge += Time.deltaTime;
                if (recharge >= startRecharge)
                {
                    StartAttack();
                }
            }
        }
        else if (!isAttacking)
        {
            speed.speed = defaultSpeed;
            anim.SetBool("Run", speed.speed > 0);
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            recharge = 0;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!isAttacking)
            {
                anim.SetBool("Attack", false);
                anim.SetBool("Run", true);
                speed.speed = defaultSpeed;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackPos.position, radius);
    }
    public void OnAttack()
    {
        Collider2D[] playerCollider = Physics2D.OverlapCircleAll(attackPos.position, radius, playerMask);
        for (int i = 0; i < playerCollider.Length; i++)
        {
            playerCollider[i].GetComponent<HealthSystem>().TakeDamage(damage, this.transform);
        }

    }
    private void StartAttack()
    {
        isAttacking = true;
        recharge = 0;
        anim.SetBool("Attack", true);
        CancelInvoke(nameof(FinishAttack));
        Invoke(nameof(FinishAttack), attackLockDuration);
    }

    private void FinishAttack()
    {
        isAttacking = false;
        anim.SetBool("Attack", false);

        if (!playerInRange)
        {
            speed.speed = defaultSpeed;
            anim.SetBool("Run", true);
        }
    }
    void UpdateAttackPointPosition()
    {
            float direction = _enemySprite.flipX ? -1f : 1f;
            float offsetX = Mathf.Abs(_attackOffset.x);
            attackPos.localPosition = new Vector3( -offsetX * direction, _attackOffset.y, 0);
    }
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        hp -= damage;
        anim.Play("Hurt");
        if (hp <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityTime;
        }
    }
    private void Die()
    {
        CancelInvoke(nameof(FinishAttack));

        GetComponent<EnemyCombat>().enabled = false;
        GetComponent<Enemy>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;

        // Переводим на слой "Dead" который не коллидирует с игроком
        gameObject.layer = LayerMask.NameToLayer("Dead");

        // Меняем все дочерние объекты тоже
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Dead");
        }

        anim.Play("Death");
        gameObject.tag = "Untagged";
    }
}
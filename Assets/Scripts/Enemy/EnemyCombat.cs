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
    [Header("Передвижние врага")]
    public Enemy speed;

    private float invincibilityTime=0.05f;
    private bool isInvincible;
    private float invincibilityTimer;

    [Header("Настройка боёвки")]
    public Collider2D enemyCollider;
    public Transform attackPos;
    public LayerMask playerMask;
    
    void Start()
    {
        Collider2D enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        _enemySprite = GetComponentInChildren<SpriteRenderer>();
        _attackOffset = attackPos.localPosition;
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
        recharge += Time.deltaTime;
        if (speed.speed > 0)
            anim.SetBool("Run", true);
    }
    public void OnTriggerStay2D(Collider2D other)
    {
            
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Run", false);
            speed.speed = 0;
            if (recharge >= startRecharge)
            {
                anim.SetBool("Attack", true);
                recharge = 0;
            }
            
        }
        else
        {

        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            recharge = 0;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Attack", false);
            anim.SetBool("Run", true);
            speed.speed = 4;
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
        anim.SetBool("Attack", false);
        for (int i = 0; i < playerCollider.Length; i++)
        {
            playerCollider[i].GetComponent<HealthSystem>().TakeDamage(damage, this.transform);
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
        {
            hp -= damage;

            if (hp <= 0)
            {
                GetComponent<EnemyCombat>().enabled = false;
                GetComponent<Enemy>().enabled = false;
                anim.Play("Death");
                gameObject.tag = "Untagged";
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Static;
                enemyCollider.enabled = false;
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
            }
        }
    }
}
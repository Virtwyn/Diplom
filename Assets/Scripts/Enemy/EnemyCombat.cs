using UnityEngine;
using UnityEngine.Windows;

public class EnemyCombat : MonoBehaviour
{
    public int hp;
    public int damage;
    public Enemy speed;
    private Transform player;
    private Animator anim;
    private Vector3 _attackOffset;
    [SerializeField] private SpriteRenderer _enemySprite;

    [SerializeField] private float invincibilityTime = 1;
    private bool isInvincible;
    private float invincibilityTimer;

    public Transform attackPos;
    public LayerMask playerMask;
    public float radius;
    public Rigidbody2D rbPlayer;

    public float recharge;
    public float startRecharge;

    void Start()
    {
        //Enemy speed = player.GetComponent<Enemy>();
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
        rbPlayer.WakeUp();
        recharge += Time.deltaTime;
        if (Enemy.speed > 0)
            anim.SetBool("Run", true);
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
            
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Run", false);
            Enemy.speed = 0;

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
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Attack", false);
            anim.SetBool("Run", true);
            Enemy.speed = 4;
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
                //Destroy(gameObject);
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
            }
        }
    }

}
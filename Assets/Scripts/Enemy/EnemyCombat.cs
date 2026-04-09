using UnityEngine;
using UnityEngine.Windows;

public class EnemyCombat : MonoBehaviour
{
    public int hp;
    public int damage;
    public Enemy speed;
    private Transform player;
    [SerializeField] private Animator anim;
    private Vector3 _attackOffset;
    [SerializeField] private SpriteRenderer _enemySprite;

    [SerializeField] private float invincibilityTime;
    private bool isInvincible;
    private float invincibilityTimer;

    public Transform attackPos;
    public LayerMask playerMask;
    public float radius;
    //public Rigidbody2D rbPlayer;
    public Collider2D enemyCollider;

    public float recharge;
    public float startRecharge;
    

    //public GameObject part;
    //[SerializeField] private float telegraphDuration = 0.3f;
    void Start()
    {
        Collider2D enemyCollider = GetComponent<Collider2D>();
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
        recharge += Time.deltaTime;
        if (speed.speed > 0)
            anim.SetBool("Run", true);
        if (hp <= 0)
        {
            //Destroy(gameObject);
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
            
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Run", false);
            speed.speed = 0;
            if (recharge >= 2.5)
            {
                //AttackSequence();
            }
            if (recharge >= startRecharge)
            {
                 
                //Instantiate(part,transform.position, Quaternion.identity);
                //part.SetActive(true);
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
        //part.SetActive(false);
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
                //Destroy(gameObject);
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
            }
        }
    }
    //System.Collections.IEnumerator AttackSequence()
    //{
    //    part.SetActive(true);
    //    part.GetComponent<ParticleSystem>().Play();
    //    yield return new WaitForSeconds(telegraphDuration);
    //    part.SetActive(false);
    //}

}
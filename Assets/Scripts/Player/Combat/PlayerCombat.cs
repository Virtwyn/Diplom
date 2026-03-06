using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform AttackPoint;
    public float AttackRange = 0.5f;
    public int AttackDamage = 40;
    public LayerMask enemyLayers;
    public float comboTime = 0.5f;
    private int comboCount = 0;  
    private float lastAttackTime;
    public float AttackRate = 1f;
    float nextAttackTime = 0f;
    [SerializeField] private SpriteRenderer _playerSprite;
     private Vector3 _attackPosition;

    private void Start()
    {
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _attackPosition=AttackPoint.position;
    }
    void Update()
    {
        if (Time.time - lastAttackTime > comboTime)
        {
            comboCount = 0;
        }
        if (Time.time >= nextAttackTime)
        {
            UpdateAttackPointPosition();
            if (Input.GetMouseButtonDown(0))
        {
            Attack();
                nextAttackTime = Time.time + 1f / AttackRate;
        }
        }
    }

    void UpdateAttackPointPosition()
    {
        if (_playerSprite != null && AttackPoint != null)
        {
            float direction = _playerSprite.flipX ? -1f : 1f;
            AttackPoint.position = transform.position + new Vector3(_attackPosition.x * direction, _attackPosition.y, 0);
        }
    }

    void Attack()
    {
        comboCount = (comboCount + 1) % 3;
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack" + (comboCount + 1));
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamege(AttackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
            return;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
}
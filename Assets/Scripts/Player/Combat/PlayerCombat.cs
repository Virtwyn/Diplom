using UnityEngine;

public class PlayerCombat : Sounds
{
    public Animator animator;
    public Transform AttackPoint;
    public float AttackRange = 0.5f;
    public int AttackDamage = 40;
    public LayerMask enemyLayers;
    public float comboTime = 0.5f;
    public CharacterMovement _speed;

    public float attackDuration = 0.6f;

    private int comboCount = 0;
    private float lastAttackTime;
    public float AttackRate = 1f;
    float nextAttackTime = 0f;

    [SerializeField] private SpriteRenderer _playerSprite;
    private CharacterMovement _movement;

    private Vector3 _attackOffset;

    private bool _isGrounded;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 _groundCheckOffset;

    CharacterMovement Move;

    private void Start()
    {
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _attackOffset = AttackPoint.localPosition;
        _movement = GetComponent<CharacterMovement>();
    }
    private void FixedUpdate()
    {
        CheckGround();
    }
    void Update()
    {
        attackDuration += Time.deltaTime;
        if (_isGrounded)
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
                    _movement = GetComponent<CharacterMovement>();
                    _movement._rigidbody.linearVelocity = new Vector2(0, _movement._rigidbody.linearVelocity.y);
                    _movement.IsAttacking = true;
                    attackDuration = 0;
                    GetComponent<CharacterMovement>().enabled = false;
                    Attack();
                    nextAttackTime = Time.time + 1f / AttackRate;
                }
            }
        }
        if (attackDuration >= 0.5f)
        {
            StopAttacking1();
        }
    }

    void UpdateAttackPointPosition()
    {
        float direction = _playerSprite.flipX ? -1f : 1f;
        AttackPoint.position = transform.position + new Vector3(_attackOffset.x * direction, _attackOffset.y, 0);
    }

    public void Attack()
    {
        //_movement._rigidbody.AddForce(transform.up * 2f, ForceMode2D.Impulse);
        comboCount = (comboCount + 1) % 3;
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack" + (comboCount + 1));

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyCombat enemyScript = enemy.GetComponent<EnemyCombat>();
            enemyScript.TakeDamage(AttackDamage);
        }
        PlaySound(sounds[0], volume: 0.05f);
        if (_movement != null && _movement.IsLunging())
        {
            return;
        }
        //Invoke(nameof(StopAttacking), attackDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
    private void CheckGround()
    {
        float rayLength = 0.1f;
        Vector3 rayStartPosition = transform.position + _groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector3.down, rayLength, groundMask);

        _isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");

        Color rayColor = _isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStartPosition, Vector3.down * rayLength, rayColor);
    }
    void StopAttacking1()
    {
        GetComponent<CharacterMovement>().enabled = true;
        _movement.IsAttacking = false;
    }
    void StopAttacking2()
    {
        GetComponent<CharacterMovement>().enabled = true;
        _movement.IsAttacking = false;
    }
}